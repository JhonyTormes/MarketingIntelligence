using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;

namespace MarketingIntelligence.Modules.Identity.Core.Identity.Repositories
{
    public interface IUserCredentialRepository
    {
        Task<UserCredential> GetByIdAsync(string userId);
        Task<UserCredential> GetByEmailAsync(string email);
        Task AddAsync(UserCredential userCredential);
    }
}
