using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Requests;
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

    public LinkShortenerController(
        ILinkRepository repository,
        IShorteningService shorteningService,
        ILogger<LinkShortenerController> logger)
    {
        _repository = repository;
        _shorteningService = shorteningService;
        _logger = logger;
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

        // Analytics (Fire and forget or minimal wait)
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        // Fallback for some proxy scenarios where ForwardedHeaders might not be fully configured or cleared
        if (string.IsNullOrEmpty(ip) || ip == "::1") 
        {
             var header = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
             if (!string.IsNullOrEmpty(header))
             {
                 ip = header.Split(',')[0].Trim();
             }
        }

        return Redirect(link.OriginalUrl);
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
        var stats = new
        {
            Link = new { link.ShortCode, link.OriginalUrl, link.CreatedAt },
            TotalClicks = clicks.Count(),
            RecentClicks = clicks.Take(50) // Limit for performance
        };

        return Ok(stats);
    }
}
