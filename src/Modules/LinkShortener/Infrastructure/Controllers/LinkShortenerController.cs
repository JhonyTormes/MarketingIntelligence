using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;
using MarketingIntelligence.Shared.Contracts;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


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
    public async Task<IActionResult> Shorten([FromBody] string originalUrl)
    {
        if (string.IsNullOrWhiteSpace(originalUrl)) // Basic validation
        {
            return BadRequest("URL cannot be empty.");
        }
        
        // TODO: Validate URL format

        // Check availability/cache of original URL - Implementation choice: Create new one every time or reuse?
        // Reuse is better for storage but requires lookup. Let's do lookup.
        var existing = await _repository.GetByOriginalUrlAsync(originalUrl);
        if (existing != null)
        {
            return Ok(new { ShortCode = existing.ShortCode, OriginalUrl = existing.OriginalUrl });
        }

        // Get next sequence ID
        var sequenceId = await _repository.GetNextSequenceIdAsync();

        // Encode to Base62
        var shortCode = _shorteningService.Encode(sequenceId);

        // Create Entity
        var newLink = new ShortenedLink(originalUrl, sequenceId, shortCode);

        // Save
        await _repository.AddAsync(newLink);
        await _repository.SaveChangesAsync();

        return CreatedAtAction(nameof(RedirectToOriginal), new { shortCode = shortCode }, new { ShortCode = shortCode, OriginalUrl = originalUrl });
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
