using MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects;
using MarketingIntelligence.Shared;


namespace MarketingIntelligence.Modules.Customers.Core.Domain.Entities
{
    public class Customer : Entity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string CompanyName { get; private set; }
        public string TaxId { get; private set; } // CPF/CNPJ para o Financeiro

        // Este objeto contém Tom de Voz, Persona, etc.
        public BrandIdentity? BrandIdentity { get; private set; }

        private Customer() { }

        public Customer(string name, string email, string companyName, string taxId)
        {
            Name = name;
            Email = email;
            CompanyName = companyName;
            TaxId = taxId;
        }

        public void UpdateBrandIdentity(BrandIdentity identity)
        {
            BrandIdentity = identity;
        }
    }
}
