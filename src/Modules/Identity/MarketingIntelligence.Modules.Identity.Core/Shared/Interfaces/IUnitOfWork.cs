namespace MarketingIntelligence.Modules.Identity.Core.Shared;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}