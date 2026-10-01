using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.Recipes;

/// <summary>
/// Deletes a comment from a recipe (recipe author only).
/// </summary>
/// <param name="RecipeId">Recipe id.</param>
/// <param name="CommentId">Comment id.</param>
/// <param name="UserId">Requesting user.</param>
public sealed record DeleteCommentCommand(RecipeId RecipeId, Guid CommentId, UserId UserId) : IRequest;
