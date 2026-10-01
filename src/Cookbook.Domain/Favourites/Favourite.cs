using Cookbook.Domain.Common;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Domain.Favourites;

/// <summary>
/// A user's bookmark of a recipe.
/// </summary>
public sealed class Favourite : AggregateRoot
{
    /// <summary>
    /// Gets the user who favourited the recipe.
    /// </summary>
    public UserId UserId { get; private set; }

    /// <summary>
    /// Gets the favourited recipe id.
    /// </summary>
    public RecipeId RecipeId { get; private set; }

    /// <summary>
    /// Gets whether the favourite is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// EF Core materialization constructor.
    /// </summary>
    private Favourite()
    {
    }

    private Favourite(Guid id, UserId userId, RecipeId recipeId, bool isActive)
        : base(id)
    {
        UserId = userId;
        RecipeId = recipeId;
        IsActive = isActive;
    }

    /// <summary>
    /// Creates an active favourite.
    /// </summary>
    /// <param name="userId">User who favourites.</param>
    /// <param name="recipeId">Recipe to favourite.</param>
    /// <param name="id">Optional fixed id for seeding.</param>
    /// <returns>A new active favourite.</returns>
    public static Favourite Activate(UserId userId, RecipeId recipeId, Guid? id = null)
        => new(id ?? Guid.NewGuid(), userId, recipeId, isActive: true);

    /// <summary>
    /// Activates the favourite mark.
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Deactivates the favourite mark.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}
