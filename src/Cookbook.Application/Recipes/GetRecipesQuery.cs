using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Query for the recipes list with visibility, search, and sort.
/// </summary>
/// <param name="ViewerId">Authenticated viewer, or null for a guest.</param>
/// <param name="Search">Optional search text.</param>
/// <param name="Sort">Sort order.</param>
public sealed record GetRecipesQuery(
    UserId? ViewerId,
    string? Search = null,
    RecipeSort Sort = RecipeSort.Newest) : IRequest<IReadOnlyList<RecipeListItemDto>>;
