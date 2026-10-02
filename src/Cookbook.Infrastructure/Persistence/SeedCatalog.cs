using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Infrastructure.Persistence;

/// <summary>
/// Extra seed recipe definitions beyond the three fixed demo recipes.
/// </summary>
internal static class SeedCatalog
{
    public static IReadOnlyList<RecipeSeed> ExtraRecipes { get; } =
    [
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd01", "Greek Salad", "Fresh Mediterranean salad", 15, Difficulty.Easy, "greek-salad.png", "lunch", Visibility.Public, true,
            [I("Cucumber", 200, MeasurementUnit.Grams), I("Tomato", 150, MeasurementUnit.Grams), I("Feta", 80, MeasurementUnit.Grams), I("Olive oil", 30, MeasurementUnit.Milliliters)],
            ["vegan", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd02", "Tomato Soup", "Comforting tomato soup", 35, Difficulty.Easy, "tomato-soup.png", "lunch", Visibility.Public, true,
            [I("Tomato", 400, MeasurementUnit.Grams), I("Onion", 100, MeasurementUnit.Grams), I("Cream", 50, MeasurementUnit.Milliliters)],
            ["quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd03", "Avocado Toast", "Simple breakfast toast", 10, Difficulty.Easy, "avocado-toast.png", "breakfast", Visibility.Public, true,
            [I("Bread", 2, MeasurementUnit.Units), I("Avocado", 1, MeasurementUnit.Units), I("Lemon juice", 10, MeasurementUnit.Milliliters)],
            ["breakfast", "quick", "vegan"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd04", "Oatmeal Bowl", "Warm oats with fruit", 12, Difficulty.Easy, "oatmeal.png", "breakfast", Visibility.Public, true,
            [I("Oats", 80, MeasurementUnit.Grams), I("Milk", 200, MeasurementUnit.Milliliters), I("Banana", 1, MeasurementUnit.Units), I("Honey", 15, MeasurementUnit.Milliliters)],
            ["breakfast"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd05", "Beef Stew", "Slow hearty stew", 90, Difficulty.Hard, "beef-stew.png", "dinner", Visibility.Public, true,
            [I("Beef", 500, MeasurementUnit.Grams), I("Potato", 300, MeasurementUnit.Grams), I("Carrot", 150, MeasurementUnit.Grams), I("Onion", 100, MeasurementUnit.Grams), I("Broth", 400, MeasurementUnit.Milliliters)],
            ["dinner"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd06", "Vegetable Curry", "Mild coconut curry", 40, Difficulty.Medium, "veg-curry.png", "dinner", Visibility.Public, true,
            [I("Chickpeas", 250, MeasurementUnit.Grams), I("Coconut milk", 200, MeasurementUnit.Milliliters), I("Spinach", 100, MeasurementUnit.Grams), I("Curry paste", 30, MeasurementUnit.Grams)],
            ["vegan", "dinner"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd07", "Pancakes", "Fluffy breakfast pancakes", 25, Difficulty.Easy, "pancakes.png", "breakfast", Visibility.Public, true,
            [I("Flour", 200, MeasurementUnit.Grams), I("Eggs", 2, MeasurementUnit.Units), I("Milk", 250, MeasurementUnit.Milliliters), I("Butter", 30, MeasurementUnit.Grams)],
            ["breakfast"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd08", "Grilled Salmon", "Lemon herb salmon", 30, Difficulty.Medium, "salmon.png", "dinner", Visibility.Public, true,
            [I("Salmon", 350, MeasurementUnit.Grams), I("Lemon", 1, MeasurementUnit.Units), I("Olive oil", 20, MeasurementUnit.Milliliters), I("Dill", 5, MeasurementUnit.Grams)],
            ["dinner", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd09", "Mushroom Risotto", "Creamy risotto", 45, Difficulty.Medium, "risotto.png", "dinner", Visibility.Public, true,
            [I("Rice", 200, MeasurementUnit.Grams), I("Mushroom", 180, MeasurementUnit.Grams), I("Parmesan", 40, MeasurementUnit.Grams), I("Broth", 500, MeasurementUnit.Milliliters)],
            ["dinner"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd10", "Caesar Salad", "Classic Caesar", 20, Difficulty.Easy, "caesar.png", "lunch", Visibility.Public, true,
            [I("Lettuce", 200, MeasurementUnit.Grams), I("Chicken", 150, MeasurementUnit.Grams), I("Parmesan", 30, MeasurementUnit.Grams), I("Croutons", 40, MeasurementUnit.Grams)],
            ["lunch", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd11", "Banana Smoothie", "Quick smoothie", 5, Difficulty.Easy, "smoothie.png", "breakfast", Visibility.Public, true,
            [I("Banana", 2, MeasurementUnit.Units), I("Milk", 250, MeasurementUnit.Milliliters), I("Yogurt", 100, MeasurementUnit.Grams), I("Honey", 10, MeasurementUnit.Milliliters)],
            ["breakfast", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd12", "Lasagna", "Layered pasta bake", 75, Difficulty.Hard, "lasagna.png", "dinner", Visibility.Public, false,
            [I("Pasta sheets", 250, MeasurementUnit.Grams), I("Minced beef", 400, MeasurementUnit.Grams), I("Tomato sauce", 300, MeasurementUnit.Milliliters), I("Cheese", 150, MeasurementUnit.Grams)],
            ["dinner"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd13", "Fish Tacos", "Crispy fish tacos", 35, Difficulty.Medium, "fish-tacos.png", "dinner", Visibility.Public, true,
            [I("White fish", 300, MeasurementUnit.Grams), I("Tortilla", 4, MeasurementUnit.Units), I("Cabbage", 100, MeasurementUnit.Grams), I("Lime", 1, MeasurementUnit.Units)],
            ["dinner", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd14", "Shakshuka", "Eggs in tomato sauce", 30, Difficulty.Easy, "shakshuka.png", "breakfast", Visibility.Public, true,
            [I("Eggs", 4, MeasurementUnit.Units), I("Tomato", 300, MeasurementUnit.Grams), I("Pepper", 100, MeasurementUnit.Grams), I("Onion", 80, MeasurementUnit.Grams)],
            ["breakfast"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd15", "Quinoa Bowl", "Protein grain bowl", 25, Difficulty.Easy, "quinoa.png", "lunch", Visibility.Public, true,
            [I("Quinoa", 150, MeasurementUnit.Grams), I("Chickpeas", 120, MeasurementUnit.Grams), I("Cucumber", 80, MeasurementUnit.Grams), I("Tahini", 20, MeasurementUnit.Milliliters)],
            ["vegan", "lunch"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd16", "Apple Pie", "Classic dessert pie", 80, Difficulty.Hard, "apple-pie.png", "dessert", Visibility.Public, false,
            [I("Apple", 600, MeasurementUnit.Grams), I("Flour", 250, MeasurementUnit.Grams), I("Butter", 150, MeasurementUnit.Grams), I("Sugar", 100, MeasurementUnit.Grams)],
            ["dessert"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd17", "Berry Yogurt Parfait", "Layered yogurt dessert", 10, Difficulty.Easy, "parfait.png", "dessert", Visibility.Public, true,
            [I("Yogurt", 200, MeasurementUnit.Grams), I("Berries", 100, MeasurementUnit.Grams), I("Granola", 50, MeasurementUnit.Grams), I("Honey", 15, MeasurementUnit.Milliliters)],
            ["dessert", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd18", "Garlic Shrimp Pasta", "Pasta with shrimp", 30, Difficulty.Medium, "shrimp-pasta.png", "dinner", Visibility.Public, true,
            [I("Spaghetti", 200, MeasurementUnit.Grams), I("Shrimp", 250, MeasurementUnit.Grams), I("Garlic", 15, MeasurementUnit.Grams), I("Olive oil", 30, MeasurementUnit.Milliliters)],
            ["dinner", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd19", "Lentil Soup", "Hearty lentil soup", 50, Difficulty.Easy, "lentil-soup.png", "lunch", Visibility.Public, false,
            [I("Lentils", 200, MeasurementUnit.Grams), I("Carrot", 100, MeasurementUnit.Grams), I("Celery", 80, MeasurementUnit.Grams), I("Broth", 700, MeasurementUnit.Milliliters)],
            ["vegan", "lunch"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd20", "Caprese Sandwich", "Tomato mozzarella sandwich", 10, Difficulty.Easy, "caprese.png", "lunch", Visibility.Public, true,
            [I("Bread", 2, MeasurementUnit.Units), I("Mozzarella", 80, MeasurementUnit.Grams), I("Tomato", 100, MeasurementUnit.Grams), I("Basil", 5, MeasurementUnit.Grams)],
            ["lunch", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd21", "Beef Burger", "Classic burger", 35, Difficulty.Medium, "burger.png", "dinner", Visibility.Public, true,
            [I("Beef patty", 200, MeasurementUnit.Grams), I("Bun", 1, MeasurementUnit.Units), I("Lettuce", 30, MeasurementUnit.Grams), I("Cheese", 30, MeasurementUnit.Grams)],
            ["dinner"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd22", "Veggie Wrap", "Fresh vegetable wrap", 15, Difficulty.Easy, "veggie-wrap.png", "lunch", Visibility.Public, true,
            [I("Tortilla", 1, MeasurementUnit.Units), I("Hummus", 40, MeasurementUnit.Grams), I("Carrot", 50, MeasurementUnit.Grams), I("Spinach", 40, MeasurementUnit.Grams)],
            ["vegan", "quick"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd23", "Tiramisu", "Coffee dessert", 40, Difficulty.Hard, "tiramisu.png", "dessert", Visibility.Private, false,
            [I("Mascarpone", 250, MeasurementUnit.Grams), I("Eggs", 3, MeasurementUnit.Units), I("Ladyfingers", 200, MeasurementUnit.Grams), I("Coffee", 120, MeasurementUnit.Milliliters)],
            ["dessert"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd24", "Ramen Bowl", "Homestyle ramen", 45, Difficulty.Medium, "ramen.png", "dinner", Visibility.Public, true,
            [I("Noodles", 200, MeasurementUnit.Grams), I("Broth", 600, MeasurementUnit.Milliliters), I("Egg", 1, MeasurementUnit.Units), I("Green onion", 20, MeasurementUnit.Grams)],
            ["dinner"]),
        Rec("dddddddd-dddd-dddd-dddd-dddddddddd25", "Stuffed Peppers", "Rice stuffed peppers", 55, Difficulty.Medium, "stuffed-peppers.png", "dinner", Visibility.Public, false,
            [I("Pepper", 3, MeasurementUnit.Units), I("Rice", 150, MeasurementUnit.Grams), I("Minced beef", 200, MeasurementUnit.Grams), I("Tomato sauce", 150, MeasurementUnit.Milliliters)],
            ["dinner"]),
    ];

    private static RecipeSeed Rec(
        string id,
        string title,
        string description,
        int minutes,
        Difficulty difficulty,
        string photo,
        string category,
        Visibility visibility,
        bool authoredByAlice,
        IngredientLine[] ingredients,
        string[] tags)
        => new(
            Guid.Parse(id),
            title,
            description,
            minutes,
            difficulty,
            photo,
            category,
            visibility,
            authoredByAlice,
            ingredients,
            tags);

    private static IngredientLine I(string name, decimal qty, MeasurementUnit unit)
        => IngredientLine.Create(name, qty, unit);

    public sealed record RecipeSeed(
        Guid Id,
        string Title,
        string Description,
        int CookingTimeMinutes,
        Difficulty Difficulty,
        string Photo,
        string Category,
        Visibility Visibility,
        bool AuthoredByAlice,
        IngredientLine[] Ingredients,
        string[] Tags);
}
