namespace MarketingIntelligence.Shared.Contracts
{
    public interface IUserLogedIn
    {
        string Name { get; }
        string Email { get; }
        DateTime LogedInAt { get; }
    }
}
