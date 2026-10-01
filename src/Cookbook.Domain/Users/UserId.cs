using Cookbook.Domain.Common;

namespace Cookbook.Domain.Users;

/// <summary>
/// Strongly typed identifier for a user.
/// </summary>
public readonly record struct UserId
{
    /// <summary>
    /// Gets the underlying identifier value.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Creates a user id from a guid.
    /// </summary>
    /// <param name="value">Non-empty guid.</param>
    public UserId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("User id cannot be empty.");
        }

        Value = value;
    }

    /// <summary>
    /// Creates a new random user id.
    /// </summary>
    /// <returns>A new user id.</returns>
    public static UserId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
