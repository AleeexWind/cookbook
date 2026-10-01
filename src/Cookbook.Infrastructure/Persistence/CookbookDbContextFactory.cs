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
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Cookbook")
            ?? throw new InvalidOperationException(
                "Set ConnectionStrings__Cookbook (for example via .env / user-secrets) before running EF tools.");

        var options = new DbContextOptionsBuilder<CookbookDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new CookbookDbContext(options);
    }
}
