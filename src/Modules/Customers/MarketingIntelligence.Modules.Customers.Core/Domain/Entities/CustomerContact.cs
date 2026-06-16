using MarketingIntelligence.Shared;

namespace MarketingIntelligence.Modules.Customers.Core.Domain.Entities;

public class CustomerContact : Entity
{
    public Guid CustomerId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public string? Role { get; private set; } // e.g. "Owner", "Manager", "Accountant"
    public bool IsMain { get; private set; }

    // EF Core constructor
    protected CustomerContact() { }

    public CustomerContact(
        Guid customerId,
        string name,
        string email,
        string? phone = null,
        string? role = null,
        bool isMain = false)
    {
        CustomerId = customerId;
        Name = name;
        Email = email;
        Phone = phone;
        Role = role;
        IsMain = isMain;
    }

    public void SetAsMain() => IsMain = true;

    public void UnsetAsMain() => IsMain = false;

    public void Update(string name, string email, string? phone = null, string? role = null)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Role = role;
    }
}
