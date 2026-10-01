using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Persistence;

/// <summary>
/// Seeds demo users, recipes, and favourites.
/// </summary>
public static class CookbookDbSeeder
{
    /// <summary>
    /// Ensures the database is migrated (or created) and seeded.
    /// </summary>
    public static async Task InitializeAsync(CookbookDbContext db, bool migrate, CancellationToken cancellationToken = default)
    {
        if (migrate)
        {
            await db.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }

        if (await db.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var alice = new UserId(SeedIds.Alice);
        var bob = new UserId(SeedIds.Bob);

        db.Users.AddRange(
            new UserAccount { Id = SeedIds.Alice, UserName = "alice" },
            new UserAccount { Id = SeedIds.Bob, UserName = "bob" });

        var carbonara = Recipe.Create(
            title: "Pasta Carbonara",
            description: "Classic Italian pasta",
            cookingTimeMinutes: 30,
            difficulty: Difficulty.Easy,
            photo: "carbonara.jpg",
            category: new Category("dinner"),
            visibility: Visibility.Public,
            authorId: alice,
            basePortions: new Portions(2),
            createdAt: new DateTimeOffset(2026, 9, 28, 0, 0, 0, TimeSpan.Zero),
            ingredients:
            [
                IngredientLine.Create("Spaghetti", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Eggs", 2, MeasurementUnit.Units),
                IngredientLine.Create("Bacon", 100, MeasurementUnit.Grams),
                IngredientLine.Create("Milk", 50, MeasurementUnit.Milliliters)
            ],
            tags: [new Tag("quick")],
            id: SeedIds.PastaCarbonara);

        carbonara.AddRating(alice, 4);
        carbonara.AddRating(bob, 5);
        carbonara.AddComment(bob, "Looks delicious", new DateTimeOffset(2026, 9, 28, 18, 0, 0, TimeSpan.Zero));
        carbonara.AddComment(alice, "Family favorite", new DateTimeOffset(2026, 9, 27, 12, 0, 0, TimeSpan.Zero));
        carbonara.AddComment(bob, "Made it twice", new DateTimeOffset(2026, 9, 26, 9, 0, 0, TimeSpan.Zero));

        var stirFry = Recipe.Create(
            title: "Chicken Stir Fry",
            description: "Quick Asian dish",
            cookingTimeMinutes: 25,
            difficulty: Difficulty.Medium,
            photo: "stirfry.jpg",
            category: new Category("lunch"),
            visibility: Visibility.Private,
            authorId: alice,
            basePortions: new Portions(2),
            createdAt: new DateTimeOffset(2026, 9, 20, 0, 0, 0, TimeSpan.Zero),
            ingredients:
            [
                IngredientLine.Create("Chicken", 300, MeasurementUnit.Grams),
                IngredientLine.Create("Rice", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Soy sauce", 30, MeasurementUnit.Milliliters)
            ],
            tags: [new Tag("quick"), new Tag("vegan")],
            id: SeedIds.ChickenStirFry);

        stirFry.AddRating(alice, 4);
        stirFry.AddComment(bob, "Nice", new DateTimeOffset(2026, 9, 19, 10, 0, 0, TimeSpan.Zero));

        var cake = Recipe.Create(
            title: "Chocolate Cake",
            description: "Rich dessert",
            cookingTimeMinutes: 60,
            difficulty: Difficulty.Hard,
            photo: "cake.jpg",
            category: new Category("dessert"),
            visibility: Visibility.Private,
            authorId: bob,
            basePortions: new Portions(2),
            createdAt: new DateTimeOffset(2026, 9, 15, 0, 0, 0, TimeSpan.Zero),
            ingredients:
            [
                IngredientLine.Create("Chocolate", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Flour", 150, MeasurementUnit.Grams),
                IngredientLine.Create("Eggs", 3, MeasurementUnit.Units)
            ],
            id: SeedIds.ChocolateCake);

        cake.AddRating(bob, 5);

        db.Recipes.AddRange(carbonara, stirFry, cake);
        db.Favourites.Add(Favourite.Activate(alice, carbonara.RecipeId));

        await db.SaveChangesAsync(cancellationToken);
    }
}
