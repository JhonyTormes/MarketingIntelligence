namespace MarketingIntelligence.Shared.Contracts;

public record LinkShortenerClickedEvent(
    Guid LinkId,
    string IpAddress,
    string UserAgent,   
    DateTime ClickedAt
);