using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;
using MarketingIntelligence.Modules.Identity.Core.Identity.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingIntelligence.Modules.Identity.Infrastructure.Persistence.Repositories
{
    public class UserCredentialRepository : IUserCredentialRepository
    {
        private readonly IdentityDbContext _dbContext;

        public UserCredentialRepository(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(UserCredential userCredential)
        {
            await _dbContext.UserCredentials.AddAsync(userCredential);
        }

        public async Task<UserCredential> GetByIdAsync(string userId)
        {
            return await _dbContext.UserCredentials.FindAsync(userId);
        }

        public async Task<UserCredential> GetByEmailAsync(string email)
        {
            return await _dbContext.UserCredentials.FirstOrDefaultAsync(uc => uc.Email == email);
        }
    }
}
