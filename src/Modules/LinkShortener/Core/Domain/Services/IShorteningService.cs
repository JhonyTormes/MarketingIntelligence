namespace MarketingIntelligence.Modules.LinkShortener.Core.Domain.Services;

public interface IShorteningService
{
    string Encode(long sequenceId);
    long Decode(string shortCode);
}
