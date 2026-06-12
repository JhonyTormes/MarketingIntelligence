using MarketingIntelligence.Shared;

namespace MarketingIntelligence.Modules.Customers.Core.Domain.Entities;

public class CustomerAddress : Entity
{
    public Guid CustomerId { get; private set; }
    public string Street { get; private set; }
    public string Number { get; private set; }
    public string? Complement { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string ZipCode { get; private set; }
    public bool IsMain { get; private set; }
    public string? Label { get; private set; } // e.g. "Commercial", "Billing", "Shipping"

    // EF Core constructor
    protected CustomerAddress() { }

    public CustomerAddress(
        Guid customerId,
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        string? complement = null,
        bool isMain = false,
        string? label = null)
    {
        CustomerId = customerId;
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
        Complement = complement;
        IsMain = isMain;
        Label = label;
    }

    public void SetAsMain() => IsMain = true;

    public void UnsetAsMain() => IsMain = false;

    public void Update(
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        string? complement = null,
        string? label = null)
    {
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        ZipCode = zipCode;
        Complement = complement;
        Label = label;
    }
}
