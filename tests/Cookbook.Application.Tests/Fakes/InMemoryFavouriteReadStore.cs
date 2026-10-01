using Cookbook.Application.Abstractions;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Application.Tests.Fakes;

internal sealed class InMemoryFavouriteReadStore : IFavouriteReadStore
{
    private readonly Dictionary<UserId, HashSet<RecipeId>> _favourites = new();

    public void Add(UserId userId, RecipeId recipeId)
    {
        if (!_favourites.TryGetValue(userId, out var set))
        {
            set = [];
            _favourites[userId] = set;
        }

        set.Add(recipeId);
    }

    public Task<IReadOnlySet<RecipeId>> GetActiveRecipeIdsAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        if (_favourites.TryGetValue(userId, out var set))
        {
            return Task.FromResult<IReadOnlySet<RecipeId>>(set);
        }

        return Task.FromResult<IReadOnlySet<RecipeId>>(new HashSet<RecipeId>());
    }
}
