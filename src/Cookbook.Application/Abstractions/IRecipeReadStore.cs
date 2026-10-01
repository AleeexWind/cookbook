using Cookbook.Domain.Recipes;

namespace Cookbook.Application.Abstractions;

/// <summary>
/// Read port for loading recipes.
/// </summary>
public interface IRecipeReadStore
{
    /// <summary>
    /// Gets all recipes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>All recipes.</returns>
    Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a recipe by id.
    /// </summary>
    /// <param name="id">Recipe id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipe, or null when not found.</returns>
    Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default);
}
