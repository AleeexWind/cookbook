using Cookbook.Application.Abstractions;
using Cookbook.Domain.Recipes;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Handles <see cref="GetRecipesQuery"/>.
/// </summary>
public sealed class GetRecipesQueryHandler(
    IRecipeReadStore recipeReadStore,
    IFavouriteReadStore favouriteReadStore) : IRequestHandler<GetRecipesQuery, IReadOnlyList<RecipeListItemDto>>
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<RecipeListItemDto>> Handle(
        GetRecipesQuery request,
        CancellationToken cancellationToken)
    {
        var recipes = await recipeReadStore.GetAllAsync(cancellationToken);

        IReadOnlySet<RecipeId> favouriteIds = request.ViewerId is { } viewerId
            ? await favouriteReadStore.GetActiveRecipeIdsAsync(viewerId, cancellationToken)
            : new HashSet<RecipeId>();

        var visible = recipes
            .Where(r => r.IsVisibleTo(request.ViewerId))
            .Where(r => RecipeSearch.Matches(r, request.Search));

        var ordered = request.Sort switch
        {
            RecipeSort.AverageRatingAsc => visible
                .OrderBy(r => r.AverageRating is null)
                .ThenBy(r => r.AverageRating)
                .ThenByDescending(r => r.CreatedAt),
            RecipeSort.AverageRatingDesc => visible
                .OrderBy(r => r.AverageRating is null)
                .ThenByDescending(r => r.AverageRating)
                .ThenByDescending(r => r.CreatedAt),
            _ => visible.OrderByDescending(r => r.CreatedAt)
        };

        return ordered
            .Select(r => Map(r, request.ViewerId is not null, favouriteIds))
            .ToList();
    }

    private static RecipeListItemDto Map(
        Recipe recipe,
        bool includeFavourite,
        IReadOnlySet<RecipeId> favouriteIds)
    {
        return new RecipeListItemDto(
            recipe.Id,
            recipe.Title,
            recipe.Description,
            recipe.CookingTimeMinutes,
            recipe.Difficulty.ToString(),
            recipe.Photo,
            recipe.Category.Value,
            recipe.Tags.Select(t => t.Value).ToList(),
            recipe.AverageRating,
            recipe.CommentsCount,
            includeFavourite ? favouriteIds.Contains(recipe.RecipeId) : null);
    }
}
