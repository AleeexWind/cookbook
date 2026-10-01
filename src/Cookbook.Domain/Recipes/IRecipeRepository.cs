namespace Cookbook.Domain.Recipes;

/// <summary>
/// Persistence port for recipes.
/// </summary>
public interface IRecipeRepository
{
    /// <summary>
    /// Gets a recipe by id.
    /// </summary>
    /// <param name="id">Recipe id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipe, or null when not found.</returns>
    Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a recipe.
    /// </summary>
    /// <param name="recipe">Recipe to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a recipe.
    /// </summary>
    /// <param name="recipe">Recipe to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default);
}
