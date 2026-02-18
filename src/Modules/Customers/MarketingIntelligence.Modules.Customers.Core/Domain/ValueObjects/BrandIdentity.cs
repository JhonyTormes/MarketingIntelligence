namespace MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects
{
    public record BrandIdentity(
        string ToneOfVoice,
        string TargetAudience,
        string[] Keywords,
        string[] Colors
    );
}
