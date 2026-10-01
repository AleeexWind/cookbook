namespace Cookbook.Domain.Recipes;

/// <summary>
/// An ingredient quantity scaled to a target portions value.
/// </summary>
/// <param name="Name">Ingredient name.</param>
/// <param name="Quantity">Scaled quantity.</param>
/// <param name="Unit">Measurement unit.</param>
public sealed record ScaledIngredient(string Name, decimal Quantity, MeasurementUnit Unit);
