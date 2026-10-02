namespace Cookbook.Application.MenuPlanning;

/// <summary>
/// A user's week menu plan.
/// </summary>
/// <param name="Portions">Shared plan portions.</param>
/// <param name="Slots">Meal slots for the week.</param>
public sealed record MenuPlanDto(int Portions, IReadOnlyList<MealSlotDto> Slots);

/// <summary>
/// One day-and-meal slot, optionally with a recipe.
/// </summary>
/// <param name="Day">Day of week name (Monday–Sunday).</param>
/// <param name="Meal">Meal type name (Breakfast, Lunch, Dinner).</param>
/// <param name="RecipeId">Placed recipe id, or null when empty.</param>
/// <param name="RecipeTitle">Placed recipe title, or null when empty.</param>
public sealed record MealSlotDto(string Day, string Meal, Guid? RecipeId, string? RecipeTitle);

/// <summary>
/// Aggregated shopping list item.
/// </summary>
/// <param name="Ingredient">Ingredient name.</param>
/// <param name="Quantity">Total quantity.</param>
/// <param name="Unit">Unit label (g, ml, units).</param>
public sealed record ShoppingListItemDto(string Ingredient, decimal Quantity, string Unit);

/// <summary>
/// Aggregated shopping list derived from a menu plan.
/// </summary>
/// <param name="Items">Shopping list lines.</param>
public sealed record ShoppingListDto(IReadOnlyList<ShoppingListItemDto> Items);
