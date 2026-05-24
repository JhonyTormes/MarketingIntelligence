using MarketingIntelligence.Modules.Identity.Core.Users.Entities;
using MarketingIntelligence.Modules.Identity.Core.Users.Repositories;

namespace MarketingIntelligence.Modules.Identity.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        public IdentityDbContext _dbContext;

        public UserRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }
    }
}
