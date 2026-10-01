using Cookbook.Domain.Recipes;
using Cookbook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IRecipeRepository"/>.
/// </summary>
public sealed class RecipeRepository(CookbookDbContext db) : IRecipeRepository
{
    /// <inheritdoc />
    public async Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        return await db.Recipes
            .Include(r => r.Ratings)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        await db.Recipes.AddAsync(recipe, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        await db.SaveChangesAsync(cancellationToken);
    }
}
