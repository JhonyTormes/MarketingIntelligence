using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;
using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketingIntelligence.Api.Controllers;

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

    [HttpGet("~/{shortCode}")] // Route override to be at root level (e.g., domain.com/{shortCode})
    public async Task<IActionResult> RedirectToOriginal(string shortCode)
    {
        // Decode logic validation (optional, could just query by shortCode directly)
        // If we query by shortCode, we don't strictly need to decode, but decoding verifies validity against our alphabet.
        // For performance, direct lookup on Indexed ShortCode is fast.

        var link = await _repository.GetByShortCodeAsync(shortCode);

        if (link == null)
        {
            return NotFound();
        }

        // Analytics (Fire and forget or minimal wait)
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        
        // Fallback for some proxy scenarios where ForwardedHeaders might not be fully configured or cleared
        if (string.IsNullOrEmpty(ip) || ip == "::1") 
        {
             var header = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
             if (!string.IsNullOrEmpty(header))
             {
                 ip = header.Split(',')[0].Trim();
             }
        }
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        // Adding click asynchronously to not block redirect? 
        // For V1, we await to ensure data integrity. Optimization can come later (Channels/BackgroundService)
        var click = new LinkClick(link.Id, ip, userAgent);
        await _repository.AddClickAsync(click);
        await _repository.SaveChangesAsync();

        return Redirect(link.OriginalUrl);
    }
}
