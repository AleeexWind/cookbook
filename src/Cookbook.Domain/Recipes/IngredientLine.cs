using Cookbook.Domain.Common;

namespace Cookbook.Domain.Recipes;

/// <summary>
/// One named ingredient on a recipe with quantity and unit for the base portions.
/// </summary>
public sealed class IngredientLine : Entity
{
    /// <summary>
    /// Gets the ingredient name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the quantity for the recipe base portions.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// Gets the measurement unit.
    /// </summary>
    public MeasurementUnit Unit { get; private set; }

    internal IngredientLine(Guid id, string name, decimal quantity, MeasurementUnit unit)
        : base(id)
    {
        Name = NormalizeName(name);
        Quantity = ValidateQuantity(quantity);
        Unit = unit;
    }

    /// <summary>
    /// Creates a new ingredient line.
    /// </summary>
    /// <param name="name">Ingredient name.</param>
    /// <param name="quantity">Positive quantity.</param>
    /// <param name="unit">Measurement unit.</param>
    /// <returns>A new ingredient line.</returns>
    public static IngredientLine Create(string name, decimal quantity, MeasurementUnit unit)
        => new(Guid.NewGuid(), name, quantity, unit);

    /// <summary>
    /// Returns the quantity scaled to the target portions.
    /// </summary>
    /// <param name="basePortions">Recipe base portions.</param>
    /// <param name="targetPortions">Desired portions.</param>
    /// <returns>Scaled quantity.</returns>
    public decimal ScaleQuantity(Portions basePortions, Portions targetPortions)
        => Quantity * targetPortions.Value / basePortions.Value;

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Ingredient name cannot be empty.");
        }

        return name.Trim();
    }

    private static decimal ValidateQuantity(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException("Ingredient quantity must be greater than zero.");
        }

        return quantity;
    }
}
