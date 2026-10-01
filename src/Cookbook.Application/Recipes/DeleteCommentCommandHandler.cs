using Cookbook.Application.Exceptions;
using Cookbook.Domain.Recipes;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Handles <see cref="DeleteCommentCommand"/>.
/// </summary>
public sealed class DeleteCommentCommandHandler(IRecipeRepository recipes) : IRequestHandler<DeleteCommentCommand>
{
    /// <inheritdoc />
    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var recipe = await recipes.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null || !recipe.IsVisibleTo(request.UserId))
        {
            throw new EntityNotFoundException("Recipe was not found.");
        }

        recipe.RemoveComment(request.CommentId, request.UserId);
        await recipes.UpdateAsync(recipe, cancellationToken);
    }
}
