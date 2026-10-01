using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Application.Abstractions;

/// <summary>
/// Read port for active favourites.
/// </summary>
public interface IFavouriteReadStore
{
    /// <summary>
    /// Gets recipe ids the user currently has favourited.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Active favourite recipe ids.</returns>
    Task<IReadOnlySet<RecipeId>> GetActiveRecipeIdsAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
