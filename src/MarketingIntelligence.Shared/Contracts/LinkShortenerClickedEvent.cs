namespace MarketingIntelligence.Shared.Contracts;

public record LinkShortenerClickedEvent(
    Guid LinkId,
    string ShortCode,
    string IpAddress,
    string UserAgent,   
    DateTime ClickedAt
);