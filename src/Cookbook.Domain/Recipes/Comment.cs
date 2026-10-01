using Cookbook.Domain.Common;
using Cookbook.Domain.Users;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// A timed text note a user leaves on a recipe.
/// </summary>
public sealed class Comment : Entity
{
    /// <summary>
    /// Gets the comment author.
    /// </summary>
    public UserId AuthorId { get; private set; }

    /// <summary>
    /// Gets the comment text.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Gets when the comment was posted.
    /// </summary>
    public DateTimeOffset PostedAt { get; private set; }

    /// <summary>
    /// EF Core materialization constructor.
    /// </summary>
    private Comment()
    {
        Text = null!;
    }

    internal Comment(Guid id, UserId authorId, string text, DateTimeOffset postedAt)
        : base(id)
    {
        AuthorId = authorId;
        Text = NormalizeText(text);
        PostedAt = postedAt;
    }

    /// <summary>
    /// Creates a comment.
    /// </summary>
    /// <param name="authorId">Comment author.</param>
    /// <param name="text">Comment text.</param>
    /// <param name="postedAt">Posted timestamp.</param>
    /// <returns>A new comment.</returns>
    public static Comment Create(UserId authorId, string text, DateTimeOffset postedAt)
        => new(Guid.NewGuid(), authorId, text, postedAt);

    private static string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new DomainException("Comment text cannot be empty.");
        }

        return text.Trim();
    }
}
