namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.DTOs;


public record LinkStatsDto(
    string ShortCode,
    string OriginalUrl,
    int TotalClicks
);