using Cookbook.Domain.Common;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// Meal-time grouping of a recipe.
/// </summary>
public readonly record struct Category
{
    /// <summary>
    /// Gets the category name.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a category from a non-empty name.
    /// </summary>
    /// <param name="value">Category name.</param>
    public Category(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Category cannot be empty.");
        }

        Value = value.Trim();
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
