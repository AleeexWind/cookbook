namespace Cookbook.Domain.Common;

/// <summary>
/// Base type for entities identified by a unique id.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// EF Core materialization constructor.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Initializes a new entity with the specified id.
    /// </summary>
    /// <param name="id">Unique identifier.</param>
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Entity id cannot be empty.");
        }

        Id = id;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id == other.Id;
    }

    /// <inheritdoc />
    public override int GetHashCode() => Id.GetHashCode();
}
