using Cookbook.Application.Abstractions;
using Cookbook.Domain.Recipes;
using Cookbook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.ReadStores;

/// <summary>
/// EF Core implementation of <see cref="IRecipeReadStore"/>.
/// </summary>
public sealed class RecipeReadStore(CookbookDbContext db) : IRecipeReadStore
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Recipe>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);
    }

    private IQueryable<Recipe> Query()
        => db.Recipes
            .AsNoTracking()
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Include(r => r.Ratings)
            .Include(r => r.Comments);
}
