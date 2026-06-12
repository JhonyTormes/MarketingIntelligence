using MarketingIntelligence.Modules.Customers.Core.Domain.Entities;
using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;

namespace MarketingIntelligence.Modules.Customers.Core.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByTaxIdAsync(string taxId);
    Task<IEnumerable<Customer>> GetAllByUserIdAsync(Guid userId);
    Task<IEnumerable<Customer>> GetAllByUserIdAsync(Guid userId, CustomerType? type = null, CustomerStatus? status = null);
    Task AddAsync(Customer customer);
    void Update(Customer customer);
    Task<bool> ExistsByTaxIdAsync(string taxId);
    Task SaveChangesAsync();
}
