using Cookbook.Domain.Common;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;

namespace Cookbook.Domain.Tests;

public sealed class MenuPlanTests
{
    [Fact]
    public void Empty_plan_has_week_slots_and_default_portions()
    {
        var plan = MenuPlan.CreateEmpty(RecipeFactory.Alice);

        Assert.Equal(2, plan.Portions.Value);
        Assert.Equal(21, plan.Slots.Count);
        Assert.False(plan.HasAnyRecipes);
        Assert.Contains(plan.Slots, s => s.Day == DayOfWeek.Monday && s.MealType == MealType.Dinner);
        Assert.Contains(plan.Slots, s => s.Day == DayOfWeek.Sunday && s.MealType == MealType.Breakfast);
    }

    [Fact]
    public void Place_and_remove_recipe_in_slot()
    {
        var plan = MenuPlan.CreateEmpty(RecipeFactory.Alice);
        var recipe = RecipeFactory.CreateCarbonara();

        plan.PlaceRecipe(DayOfWeek.Monday, MealType.Dinner, recipe.RecipeId);
        Assert.Equal(recipe.RecipeId, plan.GetSlot(DayOfWeek.Monday, MealType.Dinner).RecipeId);

        plan.RemoveFromSlot(DayOfWeek.Monday, MealType.Dinner);
        Assert.Null(plan.GetSlot(DayOfWeek.Monday, MealType.Dinner).RecipeId);
    }

    [Fact]
    public void Plan_portions_scale_recipe_ingredients()
    {
        var plan = MenuPlan.CreateEmpty(RecipeFactory.Alice);
        var recipe = RecipeFactory.CreateCarbonara(basePortions: new Portions(2));
        plan.PlaceRecipe(DayOfWeek.Monday, MealType.Dinner, recipe.RecipeId);
        plan.SetPortions(new Portions(3));

        var spaghetti = recipe.ScaleIngredients(plan.Portions).Single(i => i.Name == "Spaghetti");

        Assert.Equal(3, plan.Portions.Value);
        Assert.Equal(300, spaghetti.Quantity);
    }
}

public sealed class ShoppingListGeneratorTests
{
    private readonly ShoppingListGenerator _generator = new();

    [Fact]
    public void Generate_aggregates_ingredients_scaled_to_plan_portions()
    {
        var carbonara = RecipeFactory.CreateCarbonara();
        var stirFry = RecipeFactory.CreateStirFry();
        var plan = MenuPlan.CreateEmpty(RecipeFactory.Alice);
        plan.SetPortions(new Portions(3));
        plan.PlaceRecipe(DayOfWeek.Monday, MealType.Dinner, carbonara.RecipeId);
        plan.PlaceRecipe(DayOfWeek.Tuesday, MealType.Lunch, stirFry.RecipeId);

        var list = _generator.Generate(
            plan,
            new Dictionary<RecipeId, Recipe>
            {
                [carbonara.RecipeId] = carbonara,
                [stirFry.RecipeId] = stirFry
            });

        Assert.Equal(300, list.Items.Single(i => i.Name == "Spaghetti").Quantity);
        Assert.Equal(3, list.Items.Single(i => i.Name == "Eggs").Quantity);
        Assert.Equal(150, list.Items.Single(i => i.Name == "Bacon").Quantity);
        Assert.Equal(75, list.Items.Single(i => i.Name == "Milk").Quantity);
        Assert.Equal(450, list.Items.Single(i => i.Name == "Chicken").Quantity);
        Assert.Equal(300, list.Items.Single(i => i.Name == "Rice").Quantity);
        Assert.Equal(45, list.Items.Single(i => i.Name == "Soy sauce").Quantity);
    }

    [Fact]
    public void Duplicate_recipes_in_plan_sum_ingredient_quantities()
    {
        var carbonara = RecipeFactory.CreateCarbonara();
        var plan = MenuPlan.CreateEmpty(RecipeFactory.Alice);
        plan.SetPortions(new Portions(2));
        plan.PlaceRecipe(DayOfWeek.Monday, MealType.Dinner, carbonara.RecipeId);
        plan.PlaceRecipe(DayOfWeek.Wednesday, MealType.Breakfast, carbonara.RecipeId);

        var list = _generator.Generate(
            plan,
            new Dictionary<RecipeId, Recipe> { [carbonara.RecipeId] = carbonara });

        Assert.Equal(400, list.Items.Single(i => i.Name == "Spaghetti").Quantity);
    }

    [Fact]
    public void Empty_plan_cannot_generate_shopping_list()
    {
        var plan = MenuPlan.CreateEmpty(RecipeFactory.Alice);

        var exception = Assert.Throws<DomainException>(
            () => _generator.Generate(plan, new Dictionary<RecipeId, Recipe>()));

        Assert.Equal("Cannot generate a shopping list from an empty menu plan.", exception.Message);
    }

    [Fact]
    public void Regenerating_after_removal_drops_removed_recipe_ingredients()
    {
        var carbonara = RecipeFactory.CreateCarbonara();
        var stirFry = RecipeFactory.CreateStirFry();
        var plan = MenuPlan.CreateEmpty(RecipeFactory.Alice);
        plan.SetPortions(new Portions(2));
        plan.PlaceRecipe(DayOfWeek.Monday, MealType.Dinner, carbonara.RecipeId);
        plan.PlaceRecipe(DayOfWeek.Tuesday, MealType.Lunch, stirFry.RecipeId);

        var recipes = new Dictionary<RecipeId, Recipe>
        {
            [carbonara.RecipeId] = carbonara,
            [stirFry.RecipeId] = stirFry
        };

        _ = _generator.Generate(plan, recipes);
        plan.RemoveFromSlot(DayOfWeek.Monday, MealType.Dinner);
        var updated = _generator.Generate(plan, recipes);

        Assert.DoesNotContain(updated.Items, i => i.Name == "Spaghetti");
        Assert.Equal(300, updated.Items.Single(i => i.Name == "Chicken").Quantity);
    }
}
