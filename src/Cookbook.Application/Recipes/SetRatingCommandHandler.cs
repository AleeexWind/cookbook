using Cookbook.Application.Exceptions;
using Cookbook.Domain.Recipes;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Handles <see cref="SetRatingCommand"/>.
/// </summary>
public sealed class SetRatingCommandHandler(IRecipeRepository recipes)
    : IRequestHandler<SetRatingCommand, SetRatingResult>
{
    /// <inheritdoc />
    public async Task<SetRatingResult> Handle(SetRatingCommand request, CancellationToken cancellationToken)
    {
        var recipe = await recipes.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null || !recipe.IsVisibleTo(request.UserId))
        {
            throw new EntityNotFoundException("Recipe was not found.");
        }

        var rating = recipe.AddRating(request.UserId, request.Stars);
        await recipes.UpdateAsync(recipe, cancellationToken);

        return new SetRatingResult(rating.Stars, recipe.AverageRating);
    }
}
