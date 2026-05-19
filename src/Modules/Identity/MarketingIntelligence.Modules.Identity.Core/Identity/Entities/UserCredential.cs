using MarketingIntelligence.Shared;

namespace MarketingIntelligence.Modules.Identity.Core.Identity.Entities
{
    public class UserCredential : Entity
    {
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public bool IsTwoFactorEnabled { get; private set; }
        public bool IsLocked { get; set; }
        public DateTime LastLoginAt { get; private set; }

        protected UserCredential()
        {
        }

        public UserCredential(string email, string passwordHash) : base()
        {
            Email = email;
            PasswordHash = passwordHash;
            IsTwoFactorEnabled = false;
            IsLocked = false;
        }
    }
}
