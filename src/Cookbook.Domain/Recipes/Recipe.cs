using Cookbook.Domain.Common;
using Cookbook.Domain.Users;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// A cookable dish with characteristics, ingredients, steps, and social feedback.
/// </summary>
public sealed class Recipe : AggregateRoot
{
    private readonly List<IngredientLine> _ingredients = [];
    private readonly List<CookingStep> _steps = [];
    private readonly List<Tag> _tags = [];
    private readonly List<Rating> _ratings = [];
    private readonly List<Comment> _comments = [];

    /// <summary>
    /// Gets the strongly typed recipe id.
    /// </summary>
    public RecipeId RecipeId => new(Id);

    /// <summary>
    /// Gets the recipe title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the recipe description.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets cooking time in minutes.
    /// </summary>
    public int CookingTimeMinutes { get; private set; }

    /// <summary>
    /// Gets the difficulty.
    /// </summary>
    public Difficulty Difficulty { get; private set; }

    /// <summary>
    /// Gets the photo file name or key.
    /// </summary>
    public string Photo { get; private set; }

    /// <summary>
    /// Gets the category.
    /// </summary>
    public Category Category { get; private set; }

    /// <summary>
    /// Gets the visibility.
    /// </summary>
    public Visibility Visibility { get; private set; }

    /// <summary>
    /// Gets the author id.
    /// </summary>
    public UserId AuthorId { get; private set; }

    /// <summary>
    /// Gets the base portions the ingredient quantities are defined for.
    /// </summary>
    public Portions BasePortions { get; private set; }

    /// <summary>
    /// Gets when the recipe was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets the ingredient lines.
    /// </summary>
    public IReadOnlyList<IngredientLine> Ingredients => _ingredients;

    /// <summary>
    /// Gets the cooking steps ordered by step order.
    /// </summary>
    public IReadOnlyList<CookingStep> Steps => _steps;

    /// <summary>
    /// Gets the tags.
    /// </summary>
    public IReadOnlyList<Tag> Tags => _tags;

    /// <summary>
    /// Gets the ratings.
    /// </summary>
    public IReadOnlyList<Rating> Ratings => _ratings;

    /// <summary>
    /// Gets the comments.
    /// </summary>
    public IReadOnlyList<Comment> Comments => _comments;

    /// <summary>
    /// Gets the average rating, or null when there are no ratings.
    /// </summary>
    public decimal? AverageRating =>
        _ratings.Count == 0
            ? null
            : Math.Round((decimal)_ratings.Average(r => r.Stars), 1, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Gets the number of comments.
    /// </summary>
    public int CommentsCount => _comments.Count;

    private Recipe(
        Guid id,
        string title,
        string description,
        int cookingTimeMinutes,
        Difficulty difficulty,
        string photo,
        Category category,
        Visibility visibility,
        UserId authorId,
        Portions basePortions,
        DateTimeOffset createdAt)
        : base(id)
    {
        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);
        CookingTimeMinutes = ValidateCookingTime(cookingTimeMinutes);
        Difficulty = difficulty;
        Photo = NormalizePhoto(photo);
        Category = category;
        Visibility = visibility;
        AuthorId = authorId;
        BasePortions = basePortions;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new recipe.
    /// </summary>
    public static Recipe Create(
        string title,
        string description,
        int cookingTimeMinutes,
        Difficulty difficulty,
        string photo,
        Category category,
        Visibility visibility,
        UserId authorId,
        Portions basePortions,
        DateTimeOffset createdAt,
        IEnumerable<IngredientLine>? ingredients = null,
        IEnumerable<CookingStep>? steps = null,
        IEnumerable<Tag>? tags = null)
    {
        var recipe = new Recipe(
            Guid.NewGuid(),
            title,
            description,
            cookingTimeMinutes,
            difficulty,
            photo,
            category,
            visibility,
            authorId,
            basePortions,
            createdAt);

        if (ingredients is not null)
        {
            foreach (var ingredient in ingredients)
            {
                recipe._ingredients.Add(ingredient);
            }
        }

        if (steps is not null)
        {
            foreach (var step in steps.OrderBy(s => s.Order))
            {
                recipe._steps.Add(step);
            }
        }

        if (tags is not null)
        {
            foreach (var tag in tags)
            {
                recipe.AddTag(tag);
            }
        }

        return recipe;
    }

    /// <summary>
    /// Returns whether the given viewer may see this recipe.
    /// </summary>
    /// <param name="viewerId">Authenticated user id, or null for a guest.</param>
    /// <returns>True when the recipe is visible to the viewer.</returns>
    public bool IsVisibleTo(UserId? viewerId)
    {
        if (Visibility == Visibility.Public)
        {
            return true;
        }

        return viewerId is { } userId && userId.Equals(AuthorId);
    }

    /// <summary>
    /// Adds a tag when it is not already present.
    /// </summary>
    /// <param name="tag">Tag to add.</param>
    public void AddTag(Tag tag)
    {
        if (_tags.Contains(tag))
        {
            return;
        }

        _tags.Add(tag);
    }

    /// <summary>
    /// Adds an ingredient line.
    /// </summary>
    /// <param name="ingredient">Ingredient to add.</param>
    public void AddIngredient(IngredientLine ingredient)
    {
        ArgumentNullException.ThrowIfNull(ingredient);
        _ingredients.Add(ingredient);
    }

    /// <summary>
    /// Adds a cooking step.
    /// </summary>
    /// <param name="step">Step to add.</param>
    public void AddStep(CookingStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        _steps.Add(step);
        _steps.Sort((a, b) => a.Order.CompareTo(b.Order));
    }

    /// <summary>
    /// Adds an immutable rating for a user who has not rated yet.
    /// </summary>
    /// <param name="userId">User setting the rating.</param>
    /// <param name="stars">Stars from 1 to 5.</param>
    /// <returns>The created rating.</returns>
    public Rating AddRating(UserId userId, int stars)
    {
        if (_ratings.Any(r => r.UserId.Equals(userId)))
        {
            throw new DomainException("User has already rated this recipe.");
        }

        var rating = Rating.Create(userId, stars);
        _ratings.Add(rating);
        return rating;
    }

    /// <summary>
    /// Returns the user's rating when present.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <returns>The rating, or null.</returns>
    public Rating? GetRatingByUser(UserId userId)
        => _ratings.FirstOrDefault(r => r.UserId.Equals(userId));

    /// <summary>
    /// Adds a comment to the recipe.
    /// </summary>
    /// <param name="authorId">Comment author.</param>
    /// <param name="text">Comment text.</param>
    /// <param name="postedAt">Posted timestamp.</param>
    /// <returns>The created comment.</returns>
    public Comment AddComment(UserId authorId, string text, DateTimeOffset postedAt)
    {
        var comment = Comment.Create(authorId, text, postedAt);
        _comments.Add(comment);
        return comment;
    }

    /// <summary>
    /// Removes a comment. Only the recipe author may delete comments.
    /// </summary>
    /// <param name="commentId">Comment id.</param>
    /// <param name="requestingUserId">User requesting deletion.</param>
    public void RemoveComment(Guid commentId, UserId requestingUserId)
    {
        if (!requestingUserId.Equals(AuthorId))
        {
            throw new DomainException("Only the recipe author can delete comments.");
        }

        var comment = _comments.FirstOrDefault(c => c.Id == commentId)
            ?? throw new DomainException("Comment was not found.");

        _comments.Remove(comment);
    }

    /// <summary>
    /// Returns comments ordered from newest to oldest.
    /// </summary>
    /// <returns>Ordered comments.</returns>
    public IReadOnlyList<Comment> GetCommentsNewestFirst()
        => _comments.OrderByDescending(c => c.PostedAt).ToList();

    /// <summary>
    /// Scales ingredient quantities to the target portions.
    /// </summary>
    /// <param name="targetPortions">Desired portions.</param>
    /// <returns>Scaled ingredients.</returns>
    public IReadOnlyList<ScaledIngredient> ScaleIngredients(Portions targetPortions)
        => _ingredients
            .Select(i => new ScaledIngredient(
                i.Name,
                i.ScaleQuantity(BasePortions, targetPortions),
                i.Unit))
            .ToList();

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Recipe title cannot be empty.");
        }

        return title.Trim();
    }

    private static string NormalizeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Recipe description cannot be empty.");
        }

        return description.Trim();
    }

    private static int ValidateCookingTime(int cookingTimeMinutes)
    {
        if (cookingTimeMinutes <= 0)
        {
            throw new DomainException("Cooking time must be greater than zero.");
        }

        return cookingTimeMinutes;
    }

    private static string NormalizePhoto(string photo)
    {
        if (string.IsNullOrWhiteSpace(photo))
        {
            throw new DomainException("Recipe photo cannot be empty.");
        }

        return photo.Trim();
    }
}
