using Cookbook.Domain.Common;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Domain.MenuPlanning;

/// <summary>
/// One user's week of meal slots with a shared portions value for scaling.
/// </summary>
public sealed class MenuPlan : AggregateRoot
{
    private static readonly DayOfWeek[] WeekDays =
    [
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
        DayOfWeek.Sunday
    ];

    private static readonly MealType[] MealTypes =
    [
        MealType.Breakfast,
        MealType.Lunch,
        MealType.Dinner
    ];

    private readonly List<MealSlot> _slots = [];

    /// <summary>
    /// Gets the owner of the plan.
    /// </summary>
    public UserId OwnerId { get; private set; }

    /// <summary>
    /// Gets the shared portions used to scale planned recipes.
    /// </summary>
    public Portions Portions { get; private set; }

    /// <summary>
    /// Gets all meal slots.
    /// </summary>
    public IReadOnlyList<MealSlot> Slots => _slots;

    /// <summary>
    /// EF Core materialization constructor.
    /// </summary>
    private MenuPlan()
    {
    }

    private MenuPlan(Guid id, UserId ownerId, Portions portions, List<MealSlot> slots)
        : base(id)
    {
        OwnerId = ownerId;
        Portions = portions;
        _slots = slots;
    }

    /// <summary>
    /// Creates an empty week plan with default portions (2).
    /// </summary>
    /// <param name="ownerId">Plan owner.</param>
    /// <returns>A new empty menu plan.</returns>
    public static MenuPlan CreateEmpty(UserId ownerId)
    {
        var slots = new List<MealSlot>(WeekDays.Length * MealTypes.Length);
        foreach (var day in WeekDays)
        {
            foreach (var mealType in MealTypes)
            {
                slots.Add(MealSlot.CreateEmpty(day, mealType));
            }
        }

        return new MenuPlan(Guid.NewGuid(), ownerId, Portions.DefaultPlan, slots);
    }

    /// <summary>
    /// Gets whether any slot contains a recipe.
    /// </summary>
    public bool HasAnyRecipes => _slots.Any(s => s.HasRecipe);

    /// <summary>
    /// Sets the shared plan portions.
    /// </summary>
    /// <param name="portions">New portions.</param>
    public void SetPortions(Portions portions)
    {
        Portions = portions;
    }

    /// <summary>
    /// Places a recipe into a day and meal slot.
    /// </summary>
    /// <param name="day">Day of week.</param>
    /// <param name="mealType">Meal type.</param>
    /// <param name="recipeId">Recipe to place.</param>
    public void PlaceRecipe(DayOfWeek day, MealType mealType, RecipeId recipeId)
    {
        var slot = GetSlot(day, mealType);
        slot.Place(recipeId);
    }

    /// <summary>
    /// Removes a recipe from a day and meal slot.
    /// </summary>
    /// <param name="day">Day of week.</param>
    /// <param name="mealType">Meal type.</param>
    public void RemoveFromSlot(DayOfWeek day, MealType mealType)
    {
        var slot = GetSlot(day, mealType);
        slot.Clear();
    }

    /// <summary>
    /// Gets a slot by day and meal type.
    /// </summary>
    /// <param name="day">Day of week.</param>
    /// <param name="mealType">Meal type.</param>
    /// <returns>The meal slot.</returns>
    public MealSlot GetSlot(DayOfWeek day, MealType mealType)
        => _slots.FirstOrDefault(s => s.Day == day && s.MealType == mealType)
           ?? throw new DomainException($"Slot for {day} {mealType} was not found.");

    /// <summary>
    /// Returns distinct recipe ids currently placed in the plan.
    /// </summary>
    /// <returns>Recipe ids including duplicates across slots as separate occurrences.</returns>
    public IReadOnlyList<RecipeId> GetPlacedRecipeIds()
        => _slots
            .Where(s => s.RecipeId is not null)
            .Select(s => s.RecipeId!.Value)
            .ToList();
}
