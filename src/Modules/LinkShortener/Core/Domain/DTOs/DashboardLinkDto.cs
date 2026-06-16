namespace MarketingIntelligence.Modules.LinkShortener.Core.Domain.DTOs
{
    public record DashboardLinkDto(string CampaignName, string OriginalUrl, string ShortCode, int TotalClicks);
}
