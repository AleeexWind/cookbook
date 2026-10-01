using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Sets a one-time rating for a recipe.
/// </summary>
/// <param name="RecipeId">Recipe id.</param>
/// <param name="UserId">Authenticated user.</param>
/// <param name="Stars">Stars from 1 to 5.</param>
public sealed record SetRatingCommand(RecipeId RecipeId, UserId UserId, int Stars)
    : IRequest<SetRatingResult>;

/// <summary>
/// Result of setting a rating.
/// </summary>
/// <param name="Stars">The user's stars.</param>
/// <param name="AverageRating">Updated average rating.</param>
public sealed record SetRatingResult(int Stars, decimal? AverageRating);
