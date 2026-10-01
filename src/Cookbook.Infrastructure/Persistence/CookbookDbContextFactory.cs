using Cookbook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cookbook.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for EF migrations.
/// </summary>
public sealed class CookbookDbContextFactory : IDesignTimeDbContextFactory<CookbookDbContext>
{
    /// <inheritdoc />
    public CookbookDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=cookbook;Username=postgres;Password=postgres")
            .Options;

        return new CookbookDbContext(options);
    }
}
