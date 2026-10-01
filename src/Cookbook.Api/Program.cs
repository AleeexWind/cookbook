using Cookbook.Application;
using Cookbook.Application.Exceptions;
using Cookbook.Application.Recipes;
using Cookbook.Domain.Common;
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
    var viewerId = await CurrentUser.ResolveAsync(httpContext, users, cancellationToken);
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
    var viewerId = await CurrentUser.ResolveAsync(httpContext, users, cancellationToken);
    Portions? targetPortions = portions is null ? null : new Portions(portions.Value);
    var result = await sender.Send(
        new GetRecipeDetailsQuery(new RecipeId(id), viewerId, targetPortions),
        cancellationToken);

    return result is null ? Results.NotFound() : Results.Ok(result);
})
.WithName("GetRecipeDetails");

app.MapPut("/api/recipes/{id:guid}/favourite", async (
    Guid id,
    SetFavouriteRequest body,
    HttpContext httpContext,
    UserReadStore users,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = await CurrentUser.RequireAsync(httpContext, users, cancellationToken);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    return await ExecuteWriteAsync(async () =>
    {
        await sender.Send(new SetFavouriteCommand(new RecipeId(id), userId.Value, body.IsActive), cancellationToken);
        return Results.NoContent();
    });
})
.WithName("SetFavourite");

app.MapPut("/api/recipes/{id:guid}/rating", async (
    Guid id,
    SetRatingRequest body,
    HttpContext httpContext,
    UserReadStore users,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = await CurrentUser.RequireAsync(httpContext, users, cancellationToken);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    return await ExecuteWriteAsync(async () =>
    {
        var result = await sender.Send(new SetRatingCommand(new RecipeId(id), userId.Value, body.Stars), cancellationToken);
        return Results.Ok(result);
    });
})
.WithName("SetRating");

app.MapPost("/api/recipes/{id:guid}/comments", async (
    Guid id,
    AddCommentRequest body,
    HttpContext httpContext,
    UserReadStore users,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = await CurrentUser.RequireAsync(httpContext, users, cancellationToken);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    return await ExecuteWriteAsync(async () =>
    {
        var comment = await sender.Send(new AddCommentCommand(new RecipeId(id), userId.Value, body.Text), cancellationToken);
        return Results.Ok(comment);
    });
})
.WithName("AddComment");

app.MapDelete("/api/recipes/{id:guid}/comments/{commentId:guid}", async (
    Guid id,
    Guid commentId,
    HttpContext httpContext,
    UserReadStore users,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = await CurrentUser.RequireAsync(httpContext, users, cancellationToken);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    return await ExecuteWriteAsync(async () =>
    {
        await sender.Send(new DeleteCommentCommand(new RecipeId(id), commentId, userId.Value), cancellationToken);
        return Results.NoContent();
    });
})
.WithName("DeleteComment");

app.Run();

static async Task<IResult> ExecuteWriteAsync(Func<Task<IResult>> action)
{
    try
    {
        return await action();
    }
    catch (EntityNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
    catch (DomainException ex) when (ex.Message.Contains("already rated", StringComparison.OrdinalIgnoreCase))
    {
        return Results.Conflict(new { error = ex.Message });
    }
    catch (DomainException ex) when (ex.Message.Contains("Only the recipe author", StringComparison.OrdinalIgnoreCase))
    {
        return Results.Json(new { error = ex.Message }, statusCode: StatusCodes.Status403Forbidden);
    }
    catch (DomainException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
}

static RecipeSort ParseSort(string? sort) => sort?.Trim().ToLowerInvariant() switch
{
    "averageratingasc" => RecipeSort.AverageRatingAsc,
    "averageratingdesc" => RecipeSort.AverageRatingDesc,
    _ => RecipeSort.Newest
};

internal static class CurrentUser
{
    public static Task<UserId?> ResolveAsync(
        HttpContext httpContext,
        UserReadStore users,
        CancellationToken cancellationToken)
        => RequireAsync(httpContext, users, cancellationToken);

    public static async Task<UserId?> RequireAsync(
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
}

internal sealed record SetFavouriteRequest(bool IsActive);
internal sealed record SetRatingRequest(int Stars);
internal sealed record AddCommentRequest(string Text);

/// <summary>
/// Exposes the entry point assembly for integration tests.
/// </summary>
public partial class Program;
