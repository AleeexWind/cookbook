using Cookbook.Domain.Favourites;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure.Auth;
using Cookbook.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cookbook.Infrastructure.Persistence;

/// <summary>
/// Seeds demo users, recipes, favourites, menu plan, and optional MinIO photos.
/// </summary>
public static class CookbookDbSeeder
{
    /// <summary>
    /// Ensures the database is migrated (or created) and seeded.
    /// </summary>
    public static async Task InitializeAsync(
        CookbookDbContext db,
        bool migrate,
        IPhotoStorage? photos = null,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
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
        var passwordHasher = new PasswordHasher<UserAccount>();

        var aliceAccount = new UserAccount { Id = SeedIds.Alice, UserName = "alice" };
        aliceAccount.PasswordHash = passwordHasher.HashPassword(aliceAccount, SeedCredentials.DemoPassword);
        var bobAccount = new UserAccount { Id = SeedIds.Bob, UserName = "bob" };
        bobAccount.PasswordHash = passwordHasher.HashPassword(bobAccount, SeedCredentials.DemoPassword);

        db.Users.AddRange(aliceAccount, bobAccount);

        var carbonara = Recipe.Create(
            title: "Pasta Carbonara",
            description: "Classic Italian pasta",
            cookingTimeMinutes: 30,
            difficulty: Difficulty.Easy,
            photo: "carbonara.png",
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
            photo: "stirfry.png",
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
            photo: "cake.png",
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

        var recipes = new List<Recipe> { carbonara, stirFry, cake };
        var photoKeys = new List<string> { carbonara.Photo, stirFry.Photo, cake.Photo };
        var commentDay = 1;

        foreach (var seed in SeedCatalog.ExtraRecipes)
        {
            var author = seed.AuthoredByAlice ? alice : bob;
            var recipe = Recipe.Create(
                seed.Title,
                seed.Description,
                seed.CookingTimeMinutes,
                seed.Difficulty,
                seed.Photo,
                new Category(seed.Category),
                seed.Visibility,
                author,
                new Portions(2),
                new DateTimeOffset(2026, 8, commentDay, 0, 0, 0, TimeSpan.Zero),
                ingredients: seed.Ingredients,
                tags: seed.Tags.Select(t => new Tag(t)).ToArray(),
                id: seed.Id);

            recipe.AddRating(author, (commentDay % 5) + 1);
            if (commentDay % 2 == 0)
            {
                recipe.AddComment(
                    author == alice ? bob : alice,
                    $"Tasty: {seed.Title}",
                    new DateTimeOffset(2026, 8, commentDay, 12, 0, 0, TimeSpan.Zero));
            }

            if (commentDay % 3 == 0)
            {
                recipe.AddComment(
                    author,
                    $"Note on {seed.Title}",
                    new DateTimeOffset(2026, 8, commentDay, 15, 0, 0, TimeSpan.Zero));
            }

            recipes.Add(recipe);
            photoKeys.Add(seed.Photo);
            commentDay++;
        }

        db.Recipes.AddRange(recipes);
        db.Favourites.Add(Favourite.Activate(alice, carbonara.RecipeId));

        var plan = MenuPlan.CreateEmpty(alice);
        plan.SetPortions(new Portions(2));
        plan.PlaceRecipe(DayOfWeek.Monday, MealType.Breakfast, new RecipeId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd03")));
        plan.PlaceRecipe(DayOfWeek.Monday, MealType.Lunch, carbonara.RecipeId);
        plan.PlaceRecipe(DayOfWeek.Monday, MealType.Dinner, new RecipeId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd05")));
        plan.PlaceRecipe(DayOfWeek.Tuesday, MealType.Lunch, new RecipeId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd10")));
        plan.PlaceRecipe(DayOfWeek.Wednesday, MealType.Dinner, new RecipeId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd08")));
        plan.PlaceRecipe(DayOfWeek.Thursday, MealType.Breakfast, new RecipeId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd07")));
        plan.PlaceRecipe(DayOfWeek.Friday, MealType.Dinner, new RecipeId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd06")));
        plan.PlaceRecipe(DayOfWeek.Saturday, MealType.Lunch, new RecipeId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd15")));
        plan.PlaceRecipe(DayOfWeek.Sunday, MealType.Dinner, new RecipeId(Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd24")));
        db.MenuPlans.Add(plan);

        await db.SaveChangesAsync(cancellationToken);

        if (photos is { IsEnabled: true })
        {
            try
            {
                await photos.EnsureBucketAsync(cancellationToken);
                foreach (var key in photoKeys.Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    await using var stream = new MemoryStream(PlaceholderPng.Create(120, 90, 60));
                    await photos.UploadAsync(key, stream, "image/png", cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Failed to seed MinIO photos; continuing without photo objects.");
            }
        }
    }

    /// <summary>
    /// Seeds using services from a DI scope (API startup).
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services, bool migrate, CancellationToken cancellationToken = default)
    {
        var db = services.GetRequiredService<CookbookDbContext>();
        var photos = services.GetService<IPhotoStorage>();
        var logger = services.GetService<ILoggerFactory>()?.CreateLogger("CookbookDbSeeder");
        await InitializeAsync(db, migrate, photos, logger, cancellationToken);
    }
}
