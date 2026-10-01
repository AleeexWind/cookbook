using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Query for full recipe details including scaled ingredients and comments.
/// </summary>
/// <param name="Id">Recipe id.</param>
/// <param name="ViewerId">Authenticated viewer, or null for a guest.</param>
/// <param name="TargetPortions">Portions to scale to; null uses the recipe base portions.</param>
public sealed record GetRecipeDetailsQuery(
    RecipeId Id,
    UserId? ViewerId,
    Portions? TargetPortions = null) : IRequest<RecipeDetailsDto?>;
