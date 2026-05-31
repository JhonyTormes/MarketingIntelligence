namespace MarketingIntelligence.Shared.Contracts
{
    public record UserRegisteredEvent(
        string FirstName,
        string  LastName,
        string Email,
        DateTime RegisteredAt
        );
}
