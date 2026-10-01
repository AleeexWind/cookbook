namespace Cookbook.Domain.Recipes;

/// <summary>
/// Whether a recipe is visible to everyone or only to its author.
/// </summary>
public enum Visibility
{
    /// <summary>
    /// Visible to everyone.
    /// </summary>
    Public = 0,

    /// <summary>
    /// Visible only to the author.
    /// </summary>
    Private = 1
}
