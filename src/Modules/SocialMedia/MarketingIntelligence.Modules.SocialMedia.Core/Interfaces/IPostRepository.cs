using System.Threading;
using System.Threading.Tasks;
using MarketingIntelligence.Modules.SocialMedia.Core.Entities;

namespace MarketingIntelligence.Modules.SocialMedia.Core.Interfaces;

public interface IPostRepository
{
    Task AddAsync(Post post, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}