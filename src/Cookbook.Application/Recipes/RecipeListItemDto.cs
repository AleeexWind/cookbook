namespace Cookbook.Application.Recipes;

/// <summary>
/// A recipe card for the recipes list.
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
/// <param name="CommentsCount">Number of comments.</param>
/// <param name="IsFavourite">Favourite mark for authenticated viewers; null for guests.</param>
public sealed record RecipeListItemDto(
    Guid Id,
    string Title,
    string Description,
    int CookingTimeMinutes,
    string Difficulty,
    string Photo,
    string Category,
    IReadOnlyList<string> Tags,
    decimal? AverageRating,
    int CommentsCount,
    bool? IsFavourite);
