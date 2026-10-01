using Cookbook.Domain.Common;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// Strongly typed identifier for a recipe.
/// </summary>
public readonly record struct RecipeId
{
    /// <summary>
    /// Gets the underlying identifier value.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Creates a recipe id from a guid.
    /// </summary>
    /// <param name="value">Non-empty guid.</param>
    public RecipeId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("Recipe id cannot be empty.");
        }

        Value = value;
    }

    /// <summary>
    /// Creates a new random recipe id.
    /// </summary>
    /// <returns>A new recipe id.</returns>
    public static RecipeId New() => new(Guid.NewGuid());

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
