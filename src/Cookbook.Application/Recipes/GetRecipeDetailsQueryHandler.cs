using Cookbook.Application.Abstractions;
using Cookbook.Domain.Recipes;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Handles <see cref="GetRecipeDetailsQuery"/>.
/// </summary>
public sealed class GetRecipeDetailsQueryHandler(
    IRecipeReadStore recipeReadStore,
    IFavouriteReadStore favouriteReadStore,
    IUserReadStore userReadStore) : IRequestHandler<GetRecipeDetailsQuery, RecipeDetailsDto?>
{
    /// <inheritdoc />
    public async Task<RecipeDetailsDto?> Handle(
        GetRecipeDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var recipe = await recipeReadStore.GetByIdAsync(request.Id, cancellationToken);
        if (recipe is null || !recipe.IsVisibleTo(request.ViewerId))
        {
            return null;
        }

        var targetPortions = request.TargetPortions ?? recipe.BasePortions;
        var authorName = await userReadStore.GetDisplayNameAsync(recipe.AuthorId, cancellationToken)
            ?? recipe.AuthorId.ToString();

        bool? isFavourite = null;
        if (request.ViewerId is { } viewerId)
        {
            var favourites = await favouriteReadStore.GetActiveRecipeIdsAsync(viewerId, cancellationToken);
            isFavourite = favourites.Contains(recipe.RecipeId);
        }

        var ingredients = recipe.ScaleIngredients(targetPortions)
            .Select(i => new IngredientDto(i.Name, i.Quantity, MeasurementUnitFormatter.ToLabel(i.Unit)))
            .ToList();

        var comments = new List<CommentDto>();
        foreach (var comment in recipe.GetCommentsNewestFirst())
        {
            var commentAuthor = await userReadStore.GetDisplayNameAsync(comment.AuthorId, cancellationToken)
                ?? comment.AuthorId.ToString();
            comments.Add(new CommentDto(commentAuthor, comment.Text, comment.PostedAt));
        }

        return new RecipeDetailsDto(
            recipe.Id,
            recipe.Title,
            recipe.Description,
            recipe.CookingTimeMinutes,
            recipe.Difficulty.ToString(),
            recipe.Photo,
            recipe.Category.Value,
            recipe.Tags.Select(t => t.Value).ToList(),
            recipe.AverageRating,
            recipe.Visibility.ToString().ToLowerInvariant(),
            authorName,
            targetPortions.Value,
            ingredients,
            comments,
            isFavourite);
    }
}
