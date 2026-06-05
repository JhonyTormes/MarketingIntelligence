namespace MarketingIntelligence.Modules.LinkShortener.Infrastructure.Responses
{
    public record DashboardLinkDto(string CampaignName, string OriginalUrl, string ShortCode, int TotalClicks);
}
