using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Domain.Favourites;

/// <summary>
/// Persistence port for favourites.
/// </summary>
public interface IFavouriteRepository
{
    /// <summary>
    /// Gets a favourite for the given user and recipe.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <param name="recipeId">Recipe id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The favourite, or null when not found.</returns>
    Task<Favourite?> GetAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a favourite.
    /// </summary>
    /// <param name="favourite">Favourite to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Favourite favourite, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a favourite.
    /// </summary>
    /// <param name="favourite">Favourite to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(Favourite favourite, CancellationToken cancellationToken = default);
}
