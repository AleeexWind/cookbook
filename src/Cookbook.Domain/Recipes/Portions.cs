using Cookbook.Domain.Common;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// Number of servings a recipe or menu plan is scaled to.
/// </summary>
public readonly record struct Portions
{
    /// <summary>
    /// Gets the portions value.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Creates portions with a positive value.
    /// </summary>
    /// <param name="value">Portions greater than zero.</param>
    public Portions(int value)
    {
        if (value <= 0)
        {
            throw new DomainException("Portions must be greater than zero.");
        }

        Value = value;
    }

    /// <summary>
    /// Default plan portions (2).
    /// </summary>
    public static Portions DefaultPlan => new(2);

    /// <inheritdoc />
    public override string ToString() => Value.ToString();
}
