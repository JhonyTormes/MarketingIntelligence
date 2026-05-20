using MarketingIntelligence.Modules.Identity.Core.Identity.Entities;

namespace MarketingIntelligence.Modules.Identity.Core.Identity.Repositories
{
    public interface IUserCredentialRepository
    {
        Task<UserCredential> GetUserCredentialAsync(string userId);
        Task<UserCredential> GetUserCredentialByEmailAsync(string email);
        Task AddUserCredentialAsync(UserCredential userCredential);
    }
}
