using MarketingIntelligence.Modules.Identity.Core.Identity.Services.Interfaces;

namespace MarketingIntelligence.Modules.Identity.Infrastructure.Cryptography
{
    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
        }

        public bool Verify(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
