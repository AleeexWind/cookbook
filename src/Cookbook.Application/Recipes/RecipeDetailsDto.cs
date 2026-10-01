namespace Cookbook.Application.Recipes;

/// <summary>
/// Full recipe details for the details page.
/// </summary>
/// <param name="Id">Recipe id.</param>
/// <param name="Title">Title.</param>
/// <param name="Description">Description.</param>
/// <param name="CookingTimeMinutes">Cooking time in minutes.</param>
/// <param name="Difficulty">Difficulty label.</param>
/// <param name="Photo">Photo key or file name.</param>
/// <param name="Category">Category.</param>
/// <param name="Tags">Tags.</param>
/// <param name="AverageRating">Average rating, or null when none.</param>
/// <param name="Visibility">Visibility label.</param>
/// <param name="AuthorName">Author display name.</param>
/// <param name="Portions">Portions used for ingredient quantities.</param>
/// <param name="Ingredients">Scaled ingredients.</param>
/// <param name="Comments">Comments newest first.</param>
/// <param name="IsFavourite">Favourite mark for authenticated viewers; null for guests.</param>
public sealed record RecipeDetailsDto(
    Guid Id,
    string Title,
    string Description,
    int CookingTimeMinutes,
    string Difficulty,
    string Photo,
    string Category,
    IReadOnlyList<string> Tags,
    decimal? AverageRating,
    string Visibility,
    string AuthorName,
    int Portions,
    IReadOnlyList<IngredientDto> Ingredients,
    IReadOnlyList<CommentDto> Comments,
    bool? IsFavourite);

/// <summary>
/// An ingredient line on recipe details.
/// </summary>
/// <param name="Name">Ingredient name.</param>
/// <param name="Quantity">Scaled quantity.</param>
/// <param name="Unit">Unit label (g, units, ml).</param>
public sealed record IngredientDto(string Name, decimal Quantity, string Unit);

/// <summary>
/// A comment on recipe details.
/// </summary>
/// <param name="Id">Comment id.</param>
/// <param name="AuthorName">Author display name.</param>
/// <param name="Text">Comment text.</param>
/// <param name="PostedAt">Posted timestamp.</param>
public sealed record CommentDto(Guid Id, string AuthorName, string Text, DateTimeOffset PostedAt);
