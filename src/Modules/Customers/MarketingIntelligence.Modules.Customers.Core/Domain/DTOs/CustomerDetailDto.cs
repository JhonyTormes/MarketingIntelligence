using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;
using MarketingIntelligence.Modules.Customers.Core.Domain.ValueObjects;

namespace MarketingIntelligence.Modules.Customers.Core.Domain.DTOs;

public record AddressDto(
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

public record ContactDto(
    Guid Id,
    string Name,
    string Email,
    string? Phone,
    string? Role,
    bool IsMain
);

public record CustomerDetailDto(
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
    IReadOnlyCollection<AddressDto> Addresses,
    IReadOnlyCollection<ContactDto> Contacts,
    DateTime CreatedAt
);
