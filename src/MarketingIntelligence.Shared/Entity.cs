namespace MarketingIntelligence.Shared;

public abstract class Entity
{
    // Usando Guid sequencial para performance em SQL Server/Postgres
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    protected Entity()
    {
        Id = CreateSequentialGuid();
    }

    protected Entity(Guid id)
    {
        Id = id;
    }

    private static Guid CreateSequentialGuid()
    {
        return Guid.CreateVersion7();
    }
}