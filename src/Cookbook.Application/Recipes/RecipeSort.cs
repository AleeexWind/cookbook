namespace Cookbook.Application.Recipes;

/// <summary>
/// Sort order for the recipes list.
/// </summary>
public enum RecipeSort
{
    /// <summary>
    /// Newest created first.
    /// </summary>
    Newest = 0,

    /// <summary>
    /// Lowest average rating first; null ratings last.
    /// </summary>
    AverageRatingAsc = 1,

    /// <summary>
    /// Highest average rating first; null ratings last.
    /// </summary>
    AverageRatingDesc = 2
}
