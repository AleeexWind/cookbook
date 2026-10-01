using Cookbook.Domain.Common;
using Cookbook.Domain.Users;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// A single immutable 1–5 star score a user assigns to a recipe.
/// </summary>
public sealed class Rating : Entity
{
    /// <summary>
    /// Gets the user who set the rating.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the star score from 1 to 5.
    /// </summary>
    public int Stars { get; private set; }

    internal Rating(Guid id, UserId userId, int stars)
        : base(id)
    {
        UserId = userId;
        Stars = ValidateStars(stars);
    }

    /// <summary>
    /// Creates a rating.
    /// </summary>
    /// <param name="userId">User who rates.</param>
    /// <param name="stars">Stars from 1 to 5.</param>
    /// <returns>A new rating.</returns>
    public static Rating Create(UserId userId, int stars)
        => new(Guid.NewGuid(), userId, stars);

    private static int ValidateStars(int stars)
    {
        if (stars is < 1 or > 5)
        {
            throw new DomainException("Rating must be between 1 and 5 stars.");
        }

        return stars;
    }
}
