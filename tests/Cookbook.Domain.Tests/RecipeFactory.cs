using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Domain.Tests;

internal static class RecipeFactory
{
    public static UserId Alice { get; } = UserId.New();
    public static UserId Bob { get; } = UserId.New();

    public static Recipe CreateCarbonara(
        UserId? authorId = null,
        Visibility visibility = Visibility.Public,
        Portions? basePortions = null)
    {
        return Recipe.Create(
            title: "Pasta Carbonara",
            description: "Classic Italian pasta",
            cookingTimeMinutes: 30,
            difficulty: Difficulty.Easy,
            photo: "carbonara.jpg",
            category: new Category("dinner"),
            visibility: visibility,
            authorId: authorId ?? Alice,
            basePortions: basePortions ?? new Portions(2),
            createdAt: new DateTimeOffset(2026, 9, 28, 0, 0, 0, TimeSpan.Zero),
            ingredients:
            [
                IngredientLine.Create("Spaghetti", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Eggs", 2, MeasurementUnit.Units),
                IngredientLine.Create("Bacon", 100, MeasurementUnit.Grams),
                IngredientLine.Create("Milk", 50, MeasurementUnit.Milliliters)
            ],
            steps:
            [
                CookingStep.Create(1, "Boil the pasta."),
                CookingStep.Create(2, "Mix eggs and bacon.")
            ],
            tags: [new Tag("quick")]);
    }

    public static Recipe CreateStirFry(UserId? authorId = null, Visibility visibility = Visibility.Public)
    {
        return Recipe.Create(
            title: "Chicken Stir Fry",
            description: "Quick Asian dish",
            cookingTimeMinutes: 25,
            difficulty: Difficulty.Medium,
            photo: "stirfry.jpg",
            category: new Category("lunch"),
            visibility: visibility,
            authorId: authorId ?? Alice,
            basePortions: new Portions(2),
            createdAt: new DateTimeOffset(2026, 9, 20, 0, 0, 0, TimeSpan.Zero),
            ingredients:
            [
                IngredientLine.Create("Chicken", 300, MeasurementUnit.Grams),
                IngredientLine.Create("Rice", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Soy sauce", 30, MeasurementUnit.Milliliters)
            ],
            tags: [new Tag("quick"), new Tag("vegan")]);
    }
}
