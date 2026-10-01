using Cookbook.Application.Abstractions;
using Cookbook.Domain.Recipes;

namespace Cookbook.Application.Tests.Fakes;

internal sealed class InMemoryRecipeReadStore : IRecipeReadStore
{
    private readonly Dictionary<RecipeId, Recipe> _recipes;

    public InMemoryRecipeReadStore(IEnumerable<Recipe> recipes)
    {
        _recipes = recipes.ToDictionary(r => r.RecipeId);
    }

    public Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Recipe>>(_recipes.Values.ToList());

    public Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        _recipes.TryGetValue(id, out var recipe);
        return Task.FromResult(recipe);
    }
}
