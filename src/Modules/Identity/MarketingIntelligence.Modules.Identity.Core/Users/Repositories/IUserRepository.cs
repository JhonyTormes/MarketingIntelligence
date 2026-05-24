using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using MarketingIntelligence.Modules.Identity.Core.Users.Entities;

namespace MarketingIntelligence.Modules.Identity.Core.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId);
        Task AddAsync(User user);
    }
}
