using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for the cookbook.
/// </summary>
public sealed class CookbookDbContext(DbContextOptions<CookbookDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets the recipes set.
    /// </summary>
    public DbSet<Recipe> Recipes => Set<Recipe>();

    /// <summary>
    /// Gets the favourites set.
    /// </summary>
    public DbSet<Favourite> Favourites => Set<Favourite>();

    /// <summary>
    /// Gets the users set.
    /// </summary>
    public DbSet<UserAccount> Users => Set<UserAccount>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CookbookDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
