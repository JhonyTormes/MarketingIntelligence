namespace MarketingIntelligence.Modules.Customers.Infrastructure.Requests;

/// <summary>
/// Request payload for updating an existing customer.
/// </summary>
public record UpdateCustomerRequest
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? Notes { get; init; }

    // PF-specific
    public DateTime? BirthDate { get; init; }
    public string? Gender { get; init; }

    // PJ-specific
    public string? TradingName { get; init; }
    public string? StateRegistration { get; init; }

    // Brand Identity
    public BrandIdentityRequest? BrandIdentity { get; init; }
}
