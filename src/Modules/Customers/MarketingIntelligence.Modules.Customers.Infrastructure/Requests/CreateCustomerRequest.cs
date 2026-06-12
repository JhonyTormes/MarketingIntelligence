using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;

namespace MarketingIntelligence.Modules.Customers.Infrastructure.Requests;

/// <summary>
/// Request payload for creating a new customer (either PF or PJ).
/// </summary>
public record CreateCustomerRequest
{
    // Common
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string TaxId { get; init; } = string.Empty;
    public CustomerType Type { get; init; }
    public string? Notes { get; init; }

    // PF-specific
    public DateTime? BirthDate { get; init; }
    public string? Gender { get; init; }

    // PJ-specific
    public string? TradingName { get; init; }
    public string? StateRegistration { get; init; }

    // Brand Identity
    public BrandIdentityRequest? BrandIdentity { get; init; }

    // Addresses
    public IReadOnlyList<AddressRequest> Addresses { get; init; } = Array.Empty<AddressRequest>();

    // Contacts
    public IReadOnlyList<ContactRequest> Contacts { get; init; } = Array.Empty<ContactRequest>();
}

public record BrandIdentityRequest(
    string ToneOfVoice,
    string TargetAudience,
    string[] Keywords,
    string[] Colors
);

public record AddressRequest(
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

public record ContactRequest(
    string Name,
    string Email,
    string? Phone,
    string? Role,
    bool IsMain
);
