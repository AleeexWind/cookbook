using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Adds a comment to a recipe.
/// </summary>
/// <param name="RecipeId">Recipe id.</param>
/// <param name="UserId">Comment author.</param>
/// <param name="Text">Comment text.</param>
public sealed record AddCommentCommand(RecipeId RecipeId, UserId UserId, string Text)
    : IRequest<CommentDto>;
