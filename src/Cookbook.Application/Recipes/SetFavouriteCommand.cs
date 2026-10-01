using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Activates or deactivates a user's favourite for a recipe.
/// </summary>
/// <param name="RecipeId">Recipe id.</param>
/// <param name="UserId">Authenticated user.</param>
/// <param name="IsActive">Desired favourite state.</param>
public sealed record SetFavouriteCommand(RecipeId RecipeId, UserId UserId, bool IsActive) : IRequest;
