using MarketingIntelligence.Shared;

namespace MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;

public class ShortenedLink : Entity
{
    public string OriginalUrl { get; private set; }
    public string ShortCode { get; private set; }
    public long SequenceId { get; private set; } // O ID numérico usado para a Base62
    public Guid UserId { get; private set; }
    public string CampaignName { get; private set; }

    // EF Core constructor
    protected ShortenedLink() { }

    public ShortenedLink(string originalUrl, long sequenceId, string shortCode, Guid userId, string campaignName)
    {
        OriginalUrl = originalUrl;
        SequenceId = sequenceId;
        ShortCode = shortCode;
        UserId = userId;
        CampaignName = campaignName;
    }
}
