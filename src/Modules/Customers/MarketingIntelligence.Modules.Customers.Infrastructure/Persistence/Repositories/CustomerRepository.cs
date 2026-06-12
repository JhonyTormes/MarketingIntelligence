using MarketingIntelligence.Modules.Customers.Core.Domain.Entities;
using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;
using MarketingIntelligence.Modules.Customers.Core.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarketingIntelligence.Modules.Customers.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomersDbContext _context;

    public CustomerRepository(CustomersDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByTaxIdAsync(string taxId)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .Include(c => c.Contacts)
            .FirstOrDefaultAsync(c => c.TaxId == taxId);
    }

    public async Task<IEnumerable<Customer>> GetAllByUserIdAsync(Guid userId)
    {
        return await _context.Customers
            .Include(c => c.Addresses)
            .Include(c => c.Contacts)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Customer>> GetAllByUserIdAsync(
        Guid userId, CustomerType? type = null, CustomerStatus? status = null)
    {
        var query = _context.Customers
            .Include(c => c.Addresses)
            .Include(c => c.Contacts)
            .Where(c => c.UserId == userId);

        if (type.HasValue)
            query = query.Where(c => c.Type == type.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public void Update(Customer customer)
    {
        _context.Customers.Update(customer);
    }

    public async Task<bool> ExistsByTaxIdAsync(string taxId)
    {
        return await _context.Customers.AnyAsync(c => c.TaxId == taxId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
