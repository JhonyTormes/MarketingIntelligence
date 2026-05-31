namespace MarketingIntelligence.Shared.Contracts
{
    public record UserLogedInEvent(
        string Name,
        string Email,
        DateTime LogedInAt
    );
}
