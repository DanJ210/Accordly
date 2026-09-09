namespace Accordly.Domain.Common;

/// <summary>Base class for persisted domain entities.</summary>
public abstract class Entity
{
    /// <summary>Gets the entity identifier.</summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    /// <summary>Gets the creation timestamp.</summary>
    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
}
