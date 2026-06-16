namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Responses;


public record LinkStatsDto(
    string ShortCode,
    string OriginalUrl,
    int TotalClicks
);