using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IFavouriteRepository"/>.
/// </summary>
public sealed class FavouriteRepository(CookbookDbContext db) : IFavouriteRepository
{
    /// <inheritdoc />
    public async Task<Favourite?> GetAsync(
        UserId userId,
        RecipeId recipeId,
        CancellationToken cancellationToken = default)
    {
        var favourites = await db.Favourites.ToListAsync(cancellationToken);
        return favourites.FirstOrDefault(f => f.UserId.Equals(userId) && f.RecipeId.Equals(recipeId));
    }

    /// <inheritdoc />
    public async Task AddAsync(Favourite favourite, CancellationToken cancellationToken = default)
    {
        await db.Favourites.AddAsync(favourite, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Favourite favourite, CancellationToken cancellationToken = default)
    {
        await db.SaveChangesAsync(cancellationToken);
    }
}
