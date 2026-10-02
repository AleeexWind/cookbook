using Cookbook.Application.Abstractions;
using Cookbook.Domain.Favourites;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Infrastructure.Auth;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Infrastructure.ReadStores;
using Cookbook.Infrastructure.Repositories;
using Cookbook.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
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
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Cookbook")
            ?? throw new InvalidOperationException("Connection string 'Cookbook' is not configured.");

        services.AddDbContext<CookbookDbContext>(options =>
            options.UseNpgsql(connectionString));

        RegisterStoresAndRepositories(services);
        RegisterAuth(services, configuration);
        RegisterStorage(services, configuration);

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
        RegisterAuth(services, configuration: null);
        services.AddSingleton<IPhotoStorage, NullPhotoStorage>();
        return services;
    }

    private static void RegisterStorage(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptions.SectionName));
        var minio = configuration.GetSection(MinioOptions.SectionName).Get<MinioOptions>();
        if (minio is not null
            && !string.IsNullOrWhiteSpace(minio.AccessKey)
            && !string.IsNullOrWhiteSpace(minio.SecretKey))
        {
            services.AddSingleton<IPhotoStorage, MinioPhotoStorage>();
        }
        else
        {
            services.AddSingleton<IPhotoStorage, NullPhotoStorage>();
        }
    }

    private static void RegisterAuth(IServiceCollection services, IConfiguration? configuration)
    {
        if (configuration is not null)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        }
        else
        {
            services.Configure<JwtOptions>(options =>
            {
                options.Key = "test-signing-key-at-least-32-chars!!";
                options.Issuer = "cookbook-tests";
                options.Audience = "cookbook-tests";
                options.ExpiresMinutes = 120;
            });
        }

        services.AddSingleton<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();
        services.AddSingleton<JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
    }

    private static void RegisterStoresAndRepositories(IServiceCollection services)
    {
        services.AddScoped<IRecipeReadStore, RecipeReadStore>();
        services.AddScoped<IFavouriteReadStore, FavouriteReadStore>();
        services.AddScoped<IUserReadStore, UserReadStore>();
        services.AddScoped<UserReadStore>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IFavouriteRepository, FavouriteRepository>();
        services.AddScoped<IMenuPlanRepository, MenuPlanRepository>();
    }
}
