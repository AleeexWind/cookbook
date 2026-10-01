using Cookbook.Application.Abstractions;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.ReadStores;

/// <summary>
/// EF Core implementation of <see cref="IFavouriteReadStore"/>.
/// </summary>
public sealed class FavouriteReadStore(CookbookDbContext db) : IFavouriteReadStore
{
    /// <inheritdoc />
    public async Task<IReadOnlySet<RecipeId>> GetActiveRecipeIdsAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        var favourites = await db.Favourites
            .AsNoTracking()
            .Where(f => f.IsActive)
            .ToListAsync(cancellationToken);

        return favourites
            .Where(f => f.UserId.Equals(userId))
            .Select(f => f.RecipeId)
            .ToHashSet();
    }
}
