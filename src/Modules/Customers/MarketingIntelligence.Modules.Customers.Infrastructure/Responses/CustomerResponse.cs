using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;
using MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects;

namespace MarketingIntelligence.Modules.Customers.Infrastructure.Responses;

public record CustomerAddressResponse(
    Guid Id,
    string Street,
    string Number,
    string? Complement,
    string Neighborhood,
    string City,
    string State,
    string ZipCode,
    bool IsMain,
    string? Label
);

public record CustomerContactResponse(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? Role,
    bool IsMain
);

public record CustomerResponse(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    string TaxId,
    CustomerType Type,
    CustomerStatus Status,
    string? Notes,
    Guid UserId,
    DateTime? BirthDate,
    string? Gender,
    string? TradingName,
    string? StateRegistration,
    BrandIdentity? BrandIdentity,
    IReadOnlyCollection<CustomerAddressResponse> Addresses,
    IReadOnlyCollection<CustomerContactResponse> Contacts,
    DateTime CreatedAt
);
