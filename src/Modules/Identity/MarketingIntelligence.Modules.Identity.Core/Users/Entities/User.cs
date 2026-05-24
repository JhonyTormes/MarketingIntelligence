using MarketingIntelligence.Shared;

namespace MarketingIntelligence.Modules.Identity.Core.Users.Entities
{
    public class User : Entity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string TaxPayerId { get; private set; }
        public string PhoneNumber { get; private set; }

        protected User()
        {
        }

        public User(Guid id, string firstName, string lastName, string taxPayerId, string phoneNumber) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            TaxPayerId = taxPayerId;
            PhoneNumber = phoneNumber;
        }
    }
}
