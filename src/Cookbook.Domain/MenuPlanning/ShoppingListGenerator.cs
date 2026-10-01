using Cookbook.Domain.Common;
using Cookbook.Domain.Recipes;

namespace Cookbook.Domain.MenuPlanning;

/// <summary>
/// Generates a shopping list by scaling and aggregating ingredients from a menu plan.
/// </summary>
public sealed class ShoppingListGenerator
{
    /// <summary>
    /// Generates a shopping list from a plan and the recipes placed in it.
    /// </summary>
    /// <param name="plan">Menu plan.</param>
    /// <param name="recipes">Recipes keyed by id; must include every placed recipe.</param>
    /// <returns>Aggregated shopping list.</returns>
    public ShoppingList Generate(MenuPlan plan, IReadOnlyDictionary<RecipeId, Recipe> recipes)
    {
        ArgumentNullException.ThrowIfNull(plan);
        ArgumentNullException.ThrowIfNull(recipes);

        if (!plan.HasAnyRecipes)
        {
            throw new DomainException("Cannot generate a shopping list from an empty menu plan.");
        }

        var totals = new Dictionary<(string Name, MeasurementUnit Unit), decimal>();

        foreach (var recipeId in plan.GetPlacedRecipeIds())
        {
            if (!recipes.TryGetValue(recipeId, out var recipe))
            {
                throw new DomainException($"Recipe '{recipeId}' required by the menu plan was not found.");
            }

            foreach (var scaled in recipe.ScaleIngredients(plan.Portions))
            {
                var key = (scaled.Name, scaled.Unit);
                totals[key] = totals.TryGetValue(key, out var current)
                    ? current + scaled.Quantity
                    : scaled.Quantity;
            }
        }

        var items = totals
            .Select(pair => new ShoppingListItem(pair.Key.Name, pair.Value, pair.Key.Unit))
            .OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new ShoppingList(items);
    }
}
