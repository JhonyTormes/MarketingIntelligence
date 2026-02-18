using MarketingIntelligence.Shared;

namespace MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;

public class LinkClick : Entity
{
    public Guid ShortenedLinkId { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime ClickedAt { get; private set; }
    public string? UserAgent { get; private set; }

    // EF Core constructor
    protected LinkClick() { }

    public LinkClick(Guid linkId, string? ip, string? userAgent)
    {
        ShortenedLinkId = linkId;
        IpAddress = ip;
        UserAgent = userAgent;
        ClickedAt = DateTime.UtcNow;
    }
}
