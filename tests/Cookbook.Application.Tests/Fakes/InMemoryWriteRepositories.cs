using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Application.Tests.Fakes;

internal sealed class InMemoryRecipeRepository : IRecipeRepository
{
    private readonly Dictionary<RecipeId, Recipe> _recipes;

    public InMemoryRecipeRepository(IEnumerable<Recipe> recipes)
    {
        _recipes = recipes.ToDictionary(r => r.RecipeId);
    }

    public Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        _recipes.TryGetValue(id, out var recipe);
        return Task.FromResult(recipe);
    }

    public Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        _recipes[recipe.RecipeId] = recipe;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        _recipes[recipe.RecipeId] = recipe;
        return Task.CompletedTask;
    }
}

internal sealed class InMemoryFavouriteRepository : IFavouriteRepository
{
    private readonly List<Favourite> _favourites = [];

    public Task<Favourite?> GetAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default)
        => Task.FromResult(_favourites.FirstOrDefault(f => f.UserId.Equals(userId) && f.RecipeId.Equals(recipeId)));

    public Task AddAsync(Favourite favourite, CancellationToken cancellationToken = default)
    {
        _favourites.Add(favourite);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Favourite favourite, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
