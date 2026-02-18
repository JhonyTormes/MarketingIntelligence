namespace MarketingIntelligence.Shared;

public abstract class Entity
{
    // Usando Guid sequencial para performance em SQL Server/Postgres
    public Guid Id { get; protected set; } = CreateSequentialGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private static Guid CreateSequentialGuid()
    {
        return Guid.CreateVersion7();
    }
}