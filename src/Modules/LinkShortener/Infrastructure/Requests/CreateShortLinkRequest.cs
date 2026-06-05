namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Requests;

/// <summary>
/// Represents the incoming JSON payload for creating a new shortened link.
/// </summary>
public record CreateShortLinkRequest(
    string OriginalUrl,
    string? CampaignName
);