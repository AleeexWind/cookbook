using Cookbook.Application;
using Cookbook.Application.Recipes;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Infrastructure.ReadStores;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddApplication();
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddInfrastructure(builder.Configuration);
}

builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    if (!app.Environment.IsEnvironment("Testing"))
    {
        var db = scope.ServiceProvider.GetRequiredService<CookbookDbContext>();
        await CookbookDbSeeder.InitializeAsync(db, migrate: true);
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/recipes", async (
    string? search,
    string? sort,
    HttpContext httpContext,
    UserReadStore users,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var viewerId = await ResolveViewerAsync(httpContext, users, cancellationToken);
    var recipeSort = ParseSort(sort);
    var result = await sender.Send(new GetRecipesQuery(viewerId, search, recipeSort), cancellationToken);
    return Results.Ok(result);
})
.WithName("GetRecipes");

app.MapGet("/api/recipes/{id:guid}", async (
    Guid id,
    int? portions,
    HttpContext httpContext,
    UserReadStore users,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var viewerId = await ResolveViewerAsync(httpContext, users, cancellationToken);
    Portions? targetPortions = portions is null ? null : new Portions(portions.Value);
    var result = await sender.Send(
        new GetRecipeDetailsQuery(new RecipeId(id), viewerId, targetPortions),
        cancellationToken);

    return result is null ? Results.NotFound() : Results.Ok(result);
})
.WithName("GetRecipeDetails");

app.Run();

static async Task<UserId?> ResolveViewerAsync(
    HttpContext httpContext,
    UserReadStore users,
    CancellationToken cancellationToken)
{
    if (!httpContext.Request.Headers.TryGetValue("X-User", out var header))
    {
        return null;
    }

    var name = header.ToString().Trim();
    if (string.IsNullOrWhiteSpace(name))
    {
        return null;
    }

    return await users.FindUserIdByNameAsync(name, cancellationToken);
}

static RecipeSort ParseSort(string? sort) => sort?.Trim().ToLowerInvariant() switch
{
    "averageratingasc" => RecipeSort.AverageRatingAsc,
    "averageratingdesc" => RecipeSort.AverageRatingDesc,
    _ => RecipeSort.Newest
};

/// <summary>
/// Exposes the entry point assembly for integration tests.
/// </summary>
public partial class Program;
