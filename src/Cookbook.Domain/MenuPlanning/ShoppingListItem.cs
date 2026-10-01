using Cookbook.Domain.Recipes;

namespace Cookbook.Domain.MenuPlanning;

/// <summary>
/// One line on a shopping list: ingredient name, total quantity, and unit.
/// </summary>
/// <param name="Name">Ingredient name.</param>
/// <param name="Quantity">Aggregated quantity.</param>
/// <param name="Unit">Measurement unit.</param>
public sealed record ShoppingListItem(string Name, decimal Quantity, MeasurementUnit Unit);
