namespace MarketingIntelligence.Shared.Contracts;

public interface ILinkShortenerClicked
{
    Guid LinkId { get; }
    string IpAddress { get; }
    string UserAgent { get; }
    DateTime ClickedAt { get; }
}