using Cookbook.Application.Abstractions;
using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Infrastructure.ReadStores;
using Cookbook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Infrastructure;

/// <summary>
/// Dependency injection helpers for infrastructure.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers EF Core DbContext and read-store implementations using PostgreSQL.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Configuration containing ConnectionStrings:Cookbook.</param>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cookbook")
            ?? throw new InvalidOperationException("Connection string 'Cookbook' is not configured.");

        services.AddDbContext<CookbookDbContext>(options =>
            options.UseNpgsql(connectionString));

        RegisterStoresAndRepositories(services);

        return services;
    }

    /// <summary>
    /// Registers EF Core DbContext for tests using the provided options configuration.
    /// </summary>
    public static IServiceCollection AddInfrastructureForTests(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configure)
    {
        services.AddDbContext<CookbookDbContext>(configure);
        RegisterStoresAndRepositories(services);
        return services;
    }

    private static void RegisterStoresAndRepositories(IServiceCollection services)
    {
        services.AddScoped<IRecipeReadStore, RecipeReadStore>();
        services.AddScoped<IFavouriteReadStore, FavouriteReadStore>();
        services.AddScoped<IUserReadStore, UserReadStore>();
        services.AddScoped<UserReadStore>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IFavouriteRepository, FavouriteRepository>();
    }
}
