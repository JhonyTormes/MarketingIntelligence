using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;

namespace MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;

public interface ILinkRepository
{
    Task<ShortenedLink?> GetByShortCodeAsync(string shortCode);
    Task<ShortenedLink?> GetByOriginalUrlAsync(string originalUrl);
    Task AddAsync(ShortenedLink link);
    Task AddClickAsync(LinkClick click);
    Task<long> GetNextSequenceIdAsync();
    Task SaveChangesAsync();
}
