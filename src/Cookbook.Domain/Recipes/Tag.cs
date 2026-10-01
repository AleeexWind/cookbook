using Cookbook.Domain.Common;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// Free-form label used to find recipes.
/// </summary>
public readonly record struct Tag
{
    /// <summary>
    /// Gets the tag text.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a tag from a non-empty value.
    /// </summary>
    /// <param name="value">Tag text.</param>
    public Tag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Tag cannot be empty.");
        }

        Value = value.Trim().ToLowerInvariant();
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}
