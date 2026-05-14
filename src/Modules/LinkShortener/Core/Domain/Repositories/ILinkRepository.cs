using MarketingIntelligence.Modules.LinkShortener.Core.Domain.Entities;

namespace MarketingIntelligence.Modules.LinkShortener.Core.Domain.Repositories;

public interface ILinkRepository
{
    Task<ShortenedLink?> GetByShortCodeAsync(string shortCode);
    Task<ShortenedLink?> GetByOriginalUrlAsync(string originalUrl);
    Task AddAsync(ShortenedLink link);
    Task AddClickAsync(LinkClick click);
    Task<LinkClick?> GetClickByIdAsync(Guid id); // Optional but good practice
    Task<IEnumerable<LinkClick>> GetClicksByShortCodeAsync(string shortCode);
    Task<long> GetNextSequenceIdAsync();
    Task SaveChangesAsync();
}
