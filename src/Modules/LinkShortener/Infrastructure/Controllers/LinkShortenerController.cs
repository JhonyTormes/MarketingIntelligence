using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.DTOs;
using MarketingIntelligence.Modules.LinkShortener.Infrastructure.Requests;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
    private readonly IDistributedCache _cache;

    public LinkShortenerController(
        ILinkRepository repository,
        IShorteningService shorteningService,
        ILogger<LinkShortenerController> logger,
        IEventPublisher eventPublisher,
        IDistributedCache cache)
    {
        _repository = repository;
        _shorteningService = shorteningService;
        _logger = logger;
        _eventPublisher = eventPublisher;
        _cache = cache;
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
        string cacheKey = $"link:{shortCode}";
        string? cachedData = await _cache.GetStringAsync(cacheKey);

        string originalUrl;
        Guid linkId;

        if (!string.IsNullOrEmpty(cachedData))
        {
            var parts = cachedData.Split('|', 2);
            linkId = Guid.Parse(parts[0]);
            originalUrl = parts[1];
        }
        else
        {
            var link = await _repository.GetByShortCodeAsync(shortCode);
            if (link == null) return NotFound();

            originalUrl = link.OriginalUrl;
            linkId = link.Id;

            string dataToCache = $"{linkId}|{originalUrl}";
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            };
            await _cache.SetStringAsync(cacheKey, dataToCache, cacheOptions);
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

        var clickEvent = new LinkShortenerClickedEvent(
            linkId,
            shortCode,
            ip ?? "Desconhecido",
            userAgent ?? "Desconhecido",
            DateTime.UtcNow
        );

        await publishEndpoint.Publish(clickEvent);

        if (!originalUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            originalUrl = "https://" + originalUrl;
        }

        return Redirect(originalUrl);
    }


    [HttpGet("{shortCode}/stats")]
    [Authorize]
    public async Task<IActionResult> GetStats(string shortCode)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return Unauthorized();
        }

        var link = await _repository.GetByShortCodeAsync(shortCode);
        if (link == null || link.UserId != userId)
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
