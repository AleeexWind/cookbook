using Cookbook.Domain.Common;
using Cookbook.Domain.Recipes;

namespace Cookbook.Domain.MenuPlanning;

/// <summary>
/// A single day-and-meal position in a menu plan that may hold one recipe.
/// </summary>
public sealed class MealSlot : Entity
{
    /// <summary>
    /// Gets the day of week for the slot.
    /// </summary>
    public DayOfWeek Day { get; private set; }

    /// <summary>
    /// Gets the meal type for the slot.
    /// </summary>
    public MealType MealType { get; private set; }

    /// <summary>
    /// Gets the recipe placed in the slot, or null when empty.
    /// </summary>
    public RecipeId? RecipeId { get; private set; }

    /// <summary>
    /// EF Core materialization constructor.
    /// </summary>
    private MealSlot()
    {
    }

    internal MealSlot(Guid id, DayOfWeek day, MealType mealType, RecipeId? recipeId = null)
        : base(id)
    {
        Day = day;
        MealType = mealType;
        RecipeId = recipeId;
    }

    /// <summary>
    /// Creates an empty meal slot.
    /// </summary>
    /// <param name="day">Day of week.</param>
    /// <param name="mealType">Meal type.</param>
    /// <returns>An empty slot.</returns>
    public static MealSlot CreateEmpty(DayOfWeek day, MealType mealType)
        => new(Guid.NewGuid(), day, mealType);

    /// <summary>
    /// Gets whether the slot has a recipe.
    /// </summary>
    public bool HasRecipe => RecipeId is not null;

    /// <summary>
    /// Places a recipe into the slot.
    /// </summary>
    /// <param name="recipeId">Recipe to place.</param>
    public void Place(RecipeId recipeId)
    {
        RecipeId = recipeId;
    }

    /// <summary>
    /// Clears the recipe from the slot.
    /// </summary>
    public void Clear()
    {
        RecipeId = null;
    }
}
