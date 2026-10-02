using System.Security.Claims;
using System.Text;
using Cookbook.Application;
using Cookbook.Application.Exceptions;
using Cookbook.Application.MenuPlanning;
using Cookbook.Application.Recipes;
using Cookbook.Domain.Common;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure;
using Cookbook.Infrastructure.Auth;
using Cookbook.Infrastructure.Persistence;
using Cookbook.Infrastructure.Storage;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddApplication();
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddInfrastructure(builder.Configuration);
}

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
if (builder.Environment.IsEnvironment("Testing") && string.IsNullOrWhiteSpace(jwtOptions.Key))
{
    jwtOptions = new JwtOptions
    {
        Key = "test-signing-key-at-least-32-chars!!",
        Issuer = "cookbook-tests",
        Audience = "cookbook-tests",
        ExpiresMinutes = 120
    };
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Authorization header using the Bearer scheme."
        };
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }] = Array.Empty<string>()
        });
        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    if (!app.Environment.IsEnvironment("Testing"))
    {
        await CookbookDbSeeder.InitializeAsync(scope.ServiceProvider, migrate: true);
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/register", async (
    AuthRequest body,
    IAuthService auth,
    CancellationToken cancellationToken) =>
{
    return await ExecuteWriteAsync(async () =>
    {
        var result = await auth.RegisterAsync(body.UserName, body.Password, cancellationToken);
        return Results.Ok(result);
    });
})
.WithName("Register");

app.MapPost("/api/auth/login", async (
    AuthRequest body,
    IAuthService auth,
    CancellationToken cancellationToken) =>
{
    var result = await auth.LoginAsync(body.UserName, body.Password, cancellationToken);
    return result is null
        ? Results.Unauthorized()
        : Results.Ok(result);
})
.WithName("Login");

app.MapGet("/api/photos/{fileName}", async (
    string fileName,
    IPhotoStorage photos,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(fileName) || fileName.Contains('/') || fileName.Contains('\\') || fileName.Contains(".."))
    {
        return Results.BadRequest(new { error = "Invalid file name." });
    }

    var photo = await photos.OpenReadAsync(fileName, cancellationToken);
    if (photo is null)
    {
        return Results.NotFound();
    }

    return Results.File(photo.Value.Stream, photo.Value.ContentType);
})
.WithName("GetPhoto");

app.MapGet("/api/recipes", async (
    string? search,
    string? sort,
    HttpContext httpContext,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var viewerId = CurrentUser.Resolve(httpContext);
    var recipeSort = ParseSort(sort);
    var result = await sender.Send(new GetRecipesQuery(viewerId, search, recipeSort), cancellationToken);
    return Results.Ok(result);
})
.WithName("GetRecipes");

app.MapGet("/api/recipes/{id:guid}", async (
    Guid id,
    int? portions,
    HttpContext httpContext,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var viewerId = CurrentUser.Resolve(httpContext);
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
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
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
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
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
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
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
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
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

app.MapGet("/api/menu-plan", async (
    HttpContext httpContext,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    return await ExecuteWriteAsync(async () =>
    {
        var plan = await sender.Send(new GetMenuPlanQuery(userId.Value), cancellationToken);
        return Results.Ok(plan);
    });
})
.WithName("GetMenuPlan");

app.MapPut("/api/menu-plan/portions", async (
    SetMenuPlanPortionsRequest body,
    HttpContext httpContext,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    return await ExecuteWriteAsync(async () =>
    {
        var plan = await sender.Send(
            new SetMenuPlanPortionsCommand(userId.Value, body.Portions),
            cancellationToken);
        return Results.Ok(plan);
    });
})
.WithName("SetMenuPlanPortions");

app.MapPut("/api/menu-plan/slots/{day}/{meal}", async (
    string day,
    string meal,
    PlaceRecipeRequest body,
    HttpContext httpContext,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    if (!TryParseDay(day, out var dayOfWeek) || !TryParseMeal(meal, out var mealType))
    {
        return Results.BadRequest(new { error = "Invalid day or meal." });
    }

    return await ExecuteWriteAsync(async () =>
    {
        var plan = await sender.Send(
            new PlaceRecipeInSlotCommand(userId.Value, dayOfWeek, mealType, new RecipeId(body.RecipeId)),
            cancellationToken);
        return Results.Ok(plan);
    });
})
.WithName("PlaceRecipeInSlot");

app.MapDelete("/api/menu-plan/slots/{day}/{meal}", async (
    string day,
    string meal,
    HttpContext httpContext,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    if (!TryParseDay(day, out var dayOfWeek) || !TryParseMeal(meal, out var mealType))
    {
        return Results.BadRequest(new { error = "Invalid day or meal." });
    }

    return await ExecuteWriteAsync(async () =>
    {
        var plan = await sender.Send(
            new ClearSlotCommand(userId.Value, dayOfWeek, mealType),
            cancellationToken);
        return Results.Ok(plan);
    });
})
.WithName("ClearSlot");

app.MapGet("/api/menu-plan/shopping-list", async (
    HttpContext httpContext,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    return await ExecuteWriteAsync(async () =>
    {
        var list = await sender.Send(new GetShoppingListQuery(userId.Value), cancellationToken);
        return Results.Ok(list);
    });
})
.WithName("GetShoppingList");

app.MapGet("/api/menu-plan/shopping-list/export", async (
    HttpContext httpContext,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var userId = CurrentUser.Require(httpContext);
    if (userId is null)
    {
        return Results.Unauthorized();
    }

    return await ExecuteWriteAsync(async () =>
    {
        var list = await sender.Send(new GetShoppingListQuery(userId.Value), cancellationToken);
        var csv = ShoppingListCsvFormatter.Format(list.Items);
        return Results.File(
            System.Text.Encoding.UTF8.GetBytes(csv),
            "text/csv",
            "shopping-list.csv");
    });
})
.WithName("ExportShoppingList");

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

static bool TryParseDay(string day, out DayOfWeek dayOfWeek)
    => Enum.TryParse(day, ignoreCase: true, out dayOfWeek)
       && dayOfWeek is >= DayOfWeek.Sunday and <= DayOfWeek.Saturday;

static bool TryParseMeal(string meal, out MealType mealType)
    => Enum.TryParse(meal, ignoreCase: true, out mealType);

internal static class CurrentUser
{
    public static UserId? Resolve(HttpContext httpContext)
    {
        var value = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (value is null || !Guid.TryParse(value, out var id))
        {
            return null;
        }

        return new UserId(id);
    }

    public static UserId? Require(HttpContext httpContext) => Resolve(httpContext);
}

internal sealed record AuthRequest(string UserName, string Password);
internal sealed record SetFavouriteRequest(bool IsActive);
internal sealed record SetRatingRequest(int Stars);
internal sealed record AddCommentRequest(string Text);
internal sealed record SetMenuPlanPortionsRequest(int Portions);
internal sealed record PlaceRecipeRequest(Guid RecipeId);

/// <summary>
/// Exposes the entry point assembly for integration tests.
/// </summary>
public partial class Program;
