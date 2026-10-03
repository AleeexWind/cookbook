using Cookbook.Application.Abstractions;
using Cookbook.Application.Tests.Fakes;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Application.Tests;

internal sealed class RecipeFixtures
{
    public UserId Alice { get; } = UserId.New();
    public UserId Bob { get; } = UserId.New();

    public Recipe PastaCarbonara { get; }
    public Recipe ChickenStirFry { get; }
    public Recipe ChocolateCake { get; }

    public InMemoryRecipeReadStore Recipes { get; }
    public InMemoryFavouriteReadStore Favourites { get; }
    public InMemoryUserReadStore Users { get; }
    public ISender Sender { get; }

    public RecipeFixtures()
    {
        PastaCarbonara = Recipe.Create(
            title: "Pasta Carbonara",
            description: "Classic Italian pasta",
            cookingTimeMinutes: 30,
            difficulty: Difficulty.Easy,
            photo: "carbonara.jpg",
            category: new Category("dinner"),
            visibility: Visibility.Public,
            authorId: Alice,
            basePortions: new Portions(2),
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

        PastaCarbonara.AddRating(Alice, 4);
        PastaCarbonara.AddRating(Bob, 5);
        PastaCarbonara.AddComment(Bob, "Looks delicious", new DateTimeOffset(2026, 9, 28, 18, 0, 0, TimeSpan.Zero));
        PastaCarbonara.AddComment(Alice, "Family favorite", new DateTimeOffset(2026, 9, 27, 12, 0, 0, TimeSpan.Zero));
        PastaCarbonara.AddComment(Bob, "Made it twice", new DateTimeOffset(2026, 9, 26, 9, 0, 0, TimeSpan.Zero));

        ChickenStirFry = Recipe.Create(
            title: "Chicken Stir Fry",
            description: "Quick Asian dish",
            cookingTimeMinutes: 25,
            difficulty: Difficulty.Medium,
            photo: "stirfry.jpg",
            category: new Category("lunch"),
            visibility: Visibility.Private,
            authorId: Alice,
            basePortions: new Portions(2),
            createdAt: new DateTimeOffset(2026, 9, 20, 0, 0, 0, TimeSpan.Zero),
            ingredients:
            [
                IngredientLine.Create("Chicken", 300, MeasurementUnit.Grams),
                IngredientLine.Create("Rice", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Soy sauce", 30, MeasurementUnit.Milliliters)
            ],
            tags: [new Tag("quick"), new Tag("vegan")]);

        ChickenStirFry.AddRating(Alice, 4);
        ChickenStirFry.AddComment(Bob, "Nice", new DateTimeOffset(2026, 9, 19, 10, 0, 0, TimeSpan.Zero));

        ChocolateCake = Recipe.Create(
            title: "Chocolate Cake",
            description: "Rich dessert",
            cookingTimeMinutes: 60,
            difficulty: Difficulty.Hard,
            photo: "cake.jpg",
            category: new Category("dessert"),
            visibility: Visibility.Private,
            authorId: Bob,
            basePortions: new Portions(2),
            createdAt: new DateTimeOffset(2026, 9, 15, 0, 0, 0, TimeSpan.Zero),
            ingredients:
            [
                IngredientLine.Create("Chocolate", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Flour", 150, MeasurementUnit.Grams),
                IngredientLine.Create("Eggs", 3, MeasurementUnit.Units)
            ]);

        ChocolateCake.AddRating(Bob, 5);

        Recipes = new InMemoryRecipeReadStore([PastaCarbonara, ChickenStirFry, ChocolateCake]);
        Favourites = new InMemoryFavouriteReadStore();
        Favourites.Add(Alice, PastaCarbonara.RecipeId);
        Users = new InMemoryUserReadStore(new Dictionary<UserId, string>
        {
            [Alice] = "alice",
            [Bob] = "bob"
        });

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddSingleton<IRecipeReadStore>(Recipes);
        services.AddSingleton<IFavouriteReadStore>(Favourites);
        services.AddSingleton<IUserReadStore>(Users);
        Sender = services.BuildServiceProvider().GetRequiredService<ISender>();
    }
}
