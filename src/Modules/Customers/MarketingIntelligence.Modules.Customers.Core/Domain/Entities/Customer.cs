using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;
using MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects;
using MarketingIntelligence.Shared;

namespace MarketingIntelligence.Modules.Customers.Core.Domain.Entities;

public class Customer : Entity
{
    // Common fields for both PF (Individual) and PJ (Company)
    public string Name { get; private set; } = null!;        // Full name (PF) / Legal name (PJ)
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string TaxId { get; private set; } = null!;        // CPF (PF) / CNPJ (PJ)
    public CustomerType Type { get; private set; }
    public CustomerStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public Guid UserId { get; private set; }         // Marketing analyst who owns this customer

    // PF (Individual) specific
    public DateTime? BirthDate { get; private set; }
    public string? Gender { get; private set; }

    // PJ (Company) specific
    public string? TradingName { get; private set; }          // Nome Fantasia
    public string? StateRegistration { get; private set; }    // Inscrição Estadual

    // Brand Identity (mapped as JSON via EF Core)
    public BrandIdentity? BrandIdentity { get; private set; }

    // Navigation properties
    private readonly List<CustomerAddress> _addresses = new();
    public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();

    private readonly List<CustomerContact> _contacts = new();
    public IReadOnlyCollection<CustomerContact> Contacts => _contacts.AsReadOnly();

    // EF Core constructor
    protected Customer() { }

    private Customer(
        string name,
        string email,
        string phone,
        string taxId,
        CustomerType type,
        Guid userId,
        string? notes,
        DateTime? birthDate,
        string? gender,
        string? tradingName,
        string? stateRegistration)
    {
        Name = name;
        Email = email;
        Phone = phone;
        TaxId = taxId;
        Type = type;
        Status = CustomerStatus.Active;
        UserId = userId;
        Notes = notes;
        BirthDate = birthDate;
        Gender = gender;
        TradingName = tradingName;
        StateRegistration = stateRegistration;
    }

    // Factory: Create Individual (PF)
    public static Customer CreateIndividual(
        string fullName,
        string email,
        string phone,
        string cpf,
        Guid userId,
        string? notes = null,
        DateTime? birthDate = null,
        string? gender = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required", nameof(fullName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF is required", nameof(cpf));

        return new Customer(
            fullName,
            email,
            phone,
            cpf,
            CustomerType.Individual,
            userId,
            notes,
            birthDate,
            gender,
            tradingName: null,
            stateRegistration: null);
    }

    // Factory: Create Company (PJ)
    public static Customer CreateCompany(
        string legalName,
        string email,
        string phone,
        string cnpj,
        Guid userId,
        string? tradingName = null,
        string? stateRegistration = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(legalName))
            throw new ArgumentException("Legal name is required", nameof(legalName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ is required", nameof(cnpj));

        return new Customer(
            legalName,
            email,
            phone,
            cnpj,
            CustomerType.Company,
            userId,
            notes,
            birthDate: null,
            gender: null,
            tradingName,
            stateRegistration);
    }

    // Behavior methods
    public void UpdateDetails(
        string name,
        string email,
        string phone,
        string? notes = null,
        DateTime? birthDate = null,
        string? gender = null,
        string? tradingName = null,
        string? stateRegistration = null)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Notes = notes;

        if (Type == CustomerType.Individual)
        {
            BirthDate = birthDate;
            Gender = gender;
        }
        else if (Type == CustomerType.Company)
        {
            TradingName = tradingName;
            StateRegistration = stateRegistration;
        }
    }

    public void Activate() => Status = CustomerStatus.Active;

    public void Deactivate() => Status = CustomerStatus.Inactive;

    public void Suspend() => Status = CustomerStatus.Suspended;

    public void UpdateBrandIdentity(BrandIdentity identity)
    {
        BrandIdentity = identity;
    }

    public CustomerAddress AddAddress(
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
        var address = new CustomerAddress(Id, street, number, neighborhood, city, state, zipCode, complement, isMain, label);

        if (isMain)
        {
            foreach (var existing in _addresses)
                existing.UnsetAsMain();
        }

        _addresses.Add(address);
        return address;
    }

    public CustomerContact AddContact(
        string name,
        string email,
        string? phone = null,
        string? role = null,
        bool isMain = false)
    {
        var contact = new CustomerContact(Id, name, email, phone, role, isMain);

        if (isMain)
        {
            foreach (var existing in _contacts)
                existing.UnsetAsMain();
        }

        _contacts.Add(contact);
        return contact;
    }
}
