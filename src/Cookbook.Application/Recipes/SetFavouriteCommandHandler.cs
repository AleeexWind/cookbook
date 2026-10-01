using Cookbook.Application.Exceptions;
using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Handles <see cref="SetFavouriteCommand"/>.
/// </summary>
public sealed class SetFavouriteCommandHandler(
    IRecipeRepository recipes,
    IFavouriteRepository favourites) : IRequestHandler<SetFavouriteCommand>
{
    /// <inheritdoc />
    public async Task Handle(SetFavouriteCommand request, CancellationToken cancellationToken)
    {
        var recipe = await recipes.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null || !recipe.IsVisibleTo(request.UserId))
        {
            throw new EntityNotFoundException("Recipe was not found.");
        }

        var favourite = await favourites.GetAsync(request.UserId, request.RecipeId, cancellationToken);
        if (favourite is null)
        {
            if (!request.IsActive)
            {
                return;
            }

            await favourites.AddAsync(Favourite.Activate(request.UserId, request.RecipeId), cancellationToken);
            return;
        }

        if (request.IsActive)
        {
            favourite.Activate();
        }
        else
        {
            favourite.Deactivate();
        }

        await favourites.UpdateAsync(favourite, cancellationToken);
    }
}
