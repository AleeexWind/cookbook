using Cookbook.Application.Exceptions;
using Cookbook.Application.Abstractions;
using Cookbook.Domain.Recipes;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Handles <see cref="AddCommentCommand"/>.
/// </summary>
public sealed class AddCommentCommandHandler(
    IRecipeRepository recipes,
    IUserReadStore users,
    TimeProvider timeProvider) : IRequestHandler<AddCommentCommand, CommentDto>
{
    /// <inheritdoc />
    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var recipe = await recipes.GetByIdAsync(request.RecipeId, cancellationToken);
        if (recipe is null || !recipe.IsVisibleTo(request.UserId))
        {
            throw new EntityNotFoundException("Recipe was not found.");
        }

        var postedAt = timeProvider.GetUtcNow();
        var comment = recipe.AddComment(request.UserId, request.Text, postedAt);
        await recipes.UpdateAsync(recipe, cancellationToken);

        var authorName = await users.GetDisplayNameAsync(request.UserId, cancellationToken)
            ?? request.UserId.ToString();

        return new CommentDto(comment.Id, authorName, comment.Text, comment.PostedAt);
    }
}
