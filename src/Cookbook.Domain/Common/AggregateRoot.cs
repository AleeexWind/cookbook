namespace Cookbook.Domain.Common;

/// <summary>
/// Base type for aggregate roots.
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <summary>
    /// EF Core materialization constructor.
    /// </summary>
    protected AggregateRoot()
    {
    }

    /// <summary>
    /// Initializes a new aggregate root with the specified id.
    /// </summary>
    /// <param name="id">Unique identifier.</param>
    protected AggregateRoot(Guid id)
        : base(id)
    {
    }
}
