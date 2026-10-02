using Cookbook.Application.Exceptions;
using Cookbook.Application.MenuPlanning;
using Cookbook.Application.Tests.Fakes;
using Cookbook.Domain.Common;
using Cookbook.Domain.Favourites;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Application.Tests;

public sealed class MenuPlanCommandTests
{
    [Fact]
    public async Task GetMenuPlan_creates_empty_plan_with_default_portions()
    {
        var fx = CreateFixture();
        var plan = await fx.Sender.Send(new GetMenuPlanQuery(fx.Alice));
        Assert.Equal(2, plan.Portions);
        Assert.Equal(21, plan.Slots.Count);
        Assert.All(plan.Slots, s => Assert.Null(s.RecipeId));
    }

    [Fact]
    public async Task Place_set_portions_and_clear_slot()
    {
        var fx = CreateFixture();
        var recipe = fx.Recipes.Values.First(r => r.Title == "Pasta Carbonara");

        var placed = await fx.Sender.Send(new PlaceRecipeInSlotCommand(
            fx.Alice,
            DayOfWeek.Monday,
            MealType.Dinner,
            recipe.RecipeId));
        var mondayDinner = placed.Slots.Single(s => s.Day == "Monday" && s.Meal == "Dinner");
        Assert.Equal(recipe.Id, mondayDinner.RecipeId);
        Assert.Equal("Pasta Carbonara", mondayDinner.RecipeTitle);

        var scaled = await fx.Sender.Send(new SetMenuPlanPortionsCommand(fx.Alice, 3));
        Assert.Equal(3, scaled.Portions);

        var cleared = await fx.Sender.Send(new ClearSlotCommand(
            fx.Alice,
            DayOfWeek.Monday,
            MealType.Dinner));
        Assert.Null(cleared.Slots.Single(s => s.Day == "Monday" && s.Meal == "Dinner").RecipeId);
    }

    [Fact]
    public async Task Place_invisible_recipe_throws_not_found()
    {
        var fx = CreateFixture();
        var cake = fx.Recipes.Values.First(r => r.Title == "Chocolate Cake");

        await Assert.ThrowsAsync<EntityNotFoundException>(() => fx.Sender.Send(
            new PlaceRecipeInSlotCommand(fx.Alice, DayOfWeek.Monday, MealType.Lunch, cake.RecipeId)));
    }

    [Fact]
    public async Task Shopping_list_aggregates_and_scales()
    {
        var fx = CreateFixture();
        var carbonara = fx.Recipes.Values.First(r => r.Title == "Pasta Carbonara");
        var stirFry = fx.Recipes.Values.First(r => r.Title == "Chicken Stir Fry");

        await fx.Sender.Send(new PlaceRecipeInSlotCommand(
            fx.Alice, DayOfWeek.Monday, MealType.Dinner, carbonara.RecipeId));
        await fx.Sender.Send(new PlaceRecipeInSlotCommand(
            fx.Alice, DayOfWeek.Tuesday, MealType.Lunch, stirFry.RecipeId));
        await fx.Sender.Send(new SetMenuPlanPortionsCommand(fx.Alice, 3));

        var list = await fx.Sender.Send(new GetShoppingListQuery(fx.Alice));
        Assert.Contains(list.Items, i => i.Ingredient == "Spaghetti" && i.Quantity == 300 && i.Unit == "g");
        Assert.Contains(list.Items, i => i.Ingredient == "Chicken" && i.Quantity == 450 && i.Unit == "g");
    }

    [Fact]
    public async Task Shopping_list_empty_plan_throws()
    {
        var fx = CreateFixture();
        var ex = await Assert.ThrowsAsync<DomainException>(
            () => fx.Sender.Send(new GetShoppingListQuery(fx.Alice)));
        Assert.Contains("empty", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Csv_formatter_writes_header_and_rows()
    {
        var csv = ShoppingListCsvFormatter.Format(
        [
            new ShoppingListItemDto("Spaghetti", 200, "g"),
            new ShoppingListItemDto("Eggs", 3, "units")
        ]);

        Assert.StartsWith("Ingredient,Quantity,Unit", csv);
        Assert.Contains("Spaghetti,200,g", csv);
        Assert.Contains("Eggs,3,units", csv);
    }

    private static MenuPlanFixture CreateFixture()
    {
        var alice = UserId.New();
        var bob = UserId.New();
        var carbonara = Recipe.Create(
            "Pasta Carbonara",
            "Classic Italian pasta",
            30,
            Difficulty.Easy,
            "carbonara.jpg",
            new Category("dinner"),
            Visibility.Public,
            alice,
            new Portions(2),
            DateTimeOffset.UtcNow,
            ingredients:
            [
                IngredientLine.Create("Spaghetti", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Eggs", 2, MeasurementUnit.Units),
                IngredientLine.Create("Bacon", 100, MeasurementUnit.Grams),
                IngredientLine.Create("Milk", 50, MeasurementUnit.Milliliters)
            ]);
        var stirFry = Recipe.Create(
            "Chicken Stir Fry",
            "Quick stir fry",
            25,
            Difficulty.Medium,
            "stirfry.jpg",
            new Category("dinner"),
            Visibility.Public,
            alice,
            new Portions(2),
            DateTimeOffset.UtcNow,
            ingredients:
            [
                IngredientLine.Create("Chicken", 300, MeasurementUnit.Grams),
                IngredientLine.Create("Rice", 200, MeasurementUnit.Grams),
                IngredientLine.Create("Soy sauce", 30, MeasurementUnit.Milliliters)
            ]);
        var cake = Recipe.Create(
            "Chocolate Cake",
            "Rich dessert",
            60,
            Difficulty.Hard,
            "cake.jpg",
            new Category("dessert"),
            Visibility.Private,
            bob,
            new Portions(2),
            DateTimeOffset.UtcNow);

        var recipeRepo = new InMemoryRecipeRepository([carbonara, stirFry, cake]);
        var menuPlans = new InMemoryMenuPlanRepository();
        var readStore = new InMemoryRecipeReadStore([carbonara, stirFry, cake]);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddSingleton<IRecipeRepository>(recipeRepo);
        services.AddSingleton<IMenuPlanRepository>(menuPlans);
        services.AddSingleton<Cookbook.Application.Abstractions.IRecipeReadStore>(readStore);
        services.AddSingleton<Cookbook.Application.Abstractions.IFavouriteReadStore>(
            new InMemoryFavouriteReadStore());
        services.AddSingleton<Cookbook.Application.Abstractions.IUserReadStore>(
            new InMemoryUserReadStore(new Dictionary<UserId, string>
            {
                [alice] = "alice",
                [bob] = "bob"
            }));
        services.AddSingleton<IFavouriteRepository>(new InMemoryFavouriteRepository());

        return new MenuPlanFixture(
            alice,
            bob,
            new Dictionary<RecipeId, Recipe>
            {
                [carbonara.RecipeId] = carbonara,
                [stirFry.RecipeId] = stirFry,
                [cake.RecipeId] = cake
            },
            services.BuildServiceProvider().GetRequiredService<ISender>());
    }

    private sealed record MenuPlanFixture(
        UserId Alice,
        UserId Bob,
        Dictionary<RecipeId, Recipe> Recipes,
        ISender Sender);
}
