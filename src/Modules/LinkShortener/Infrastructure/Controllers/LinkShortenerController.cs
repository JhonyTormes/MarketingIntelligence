using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.DTOs;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Requests;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Controllers;

[ApiController]
[Route("api/links")]
public class LinkShortenerController : ControllerBase
{
    private readonly ILinkRepository _repository;
    private readonly IShorteningService _shorteningService;
    private readonly ILogger<LinkShortenerController> _logger;
    private readonly IEventPublisher _eventPublisher;

    public LinkShortenerController(
        ILinkRepository repository,
        IShorteningService shorteningService,
        ILogger<LinkShortenerController> logger,
        IEventPublisher eventPublisher)
    {
        _repository = repository;
        _shorteningService = shorteningService;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    [HttpPost]
    [Authorize] 
    public async Task<IActionResult> Shorten([FromBody] CreateShortLinkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OriginalUrl))
        {
            return BadRequest("URL cannot be empty.");
        }

        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return Unauthorized();
        }

        var sequenceId = await _repository.GetNextSequenceIdAsync();
        var shortCode = _shorteningService.Encode(sequenceId);

        var newLink = new ShortenedLink(
            request.OriginalUrl,
            sequenceId,
            shortCode,
            userId,
            request.CampaignName
        );

        await _repository.AddAsync(newLink);
        await _repository.SaveChangesAsync();

        return CreatedAtAction(
            nameof(RedirectToOriginal),
            new { shortCode = shortCode },
            new { ShortCode = shortCode, OriginalUrl = request.OriginalUrl, CampaignName = request.CampaignName }
        );
    }

    [HttpGet("~/{shortCode}")]
    public async Task<IActionResult> RedirectToOriginal(string shortCode, [FromServices] IPublishEndpoint publishEndpoint)
    {
        var link = await _repository.GetByShortCodeAsync(shortCode);
        if (link == null) return NotFound();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        if (string.IsNullOrEmpty(ip) || ip == "::1") 
        {
             var header = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
             if (!string.IsNullOrEmpty(header))
             {
                 ip = header.Split(',')[0].Trim();
             }
        }

        var clickEvent = new LinkShortenerClickedEvent(
            link.Id,
            ip ?? "Desconhecido",
            userAgent ?? "Desconhecido",
            DateTime.UtcNow
        );

        await _eventPublisher.PublishAsync(clickEvent);

        string linkReturn = string.Empty;
        if (!link.OriginalUrl.StartsWith("http"))
            linkReturn = "https://";

        linkReturn += link.OriginalUrl;

        return Redirect(linkReturn);
    }


    [HttpGet("{shortCode}/stats")]
    [Authorize]
    public async Task<IActionResult> GetStats(string shortCode)
    {
        var link = await _repository.GetByShortCodeAsync(shortCode);
        if (link == null)
        {
            return NotFound();
        }

        var clicks = await _repository.GetClicksByShortCodeAsync(shortCode);

        // Map to DTO if needed, for now returning entities (internal use) or anonymous
        var responseDto = new LinkStatsDto(
          ShortCode: link.ShortCode,
          OriginalUrl: link.OriginalUrl,
          TotalClicks: clicks.Count()
      );

        return Ok(responseDto);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMyLinks()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return Unauthorized();
        }

        var myLinks = await _repository.GetDashboardLinksAsync(userId);

        var responseData = myLinks.Select(link => new
        {
            campaign = link.CampaignName ?? "Sem Campanha",
            originalUrl = link.OriginalUrl,
            shortUrl = $"https://localhost:7118/{link.ShortCode}",
            clicks = link.TotalClicks
        });

        return Ok(responseData);
    }
}
