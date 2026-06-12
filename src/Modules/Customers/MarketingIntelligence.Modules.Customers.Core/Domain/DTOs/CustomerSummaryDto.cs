using MarketingIntelligence.Modules.Customers.Core.Domain.Enums;

namespace MarketingIntelligence.Modules.Customers.Core.Domain.DTOs;

public record CustomerSummaryDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    string TaxId,
    CustomerType Type,
    CustomerStatus Status,
    string? TradingName,
    int AddressCount,
    int ContactCount,
    DateTime CreatedAt
);
