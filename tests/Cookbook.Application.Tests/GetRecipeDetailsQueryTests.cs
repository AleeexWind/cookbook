using Cookbook.Application.Recipes;
using Cookbook.Domain.Recipes;

namespace Cookbook.Application.Tests;

public sealed class GetRecipeDetailsQueryTests
{
    [Fact]
    public async Task Guest_views_public_details_without_favourites_mark()
    {
        var fx = new RecipeFixtures();

        var details = await fx.Sender.Send(new GetRecipeDetailsQuery(fx.PastaCarbonara.RecipeId, ViewerId: null));

        Assert.NotNull(details);
        Assert.Equal("Pasta Carbonara", details.Title);
        Assert.Equal("alice", details.AuthorName);
        Assert.Equal("public", details.Visibility);
        Assert.Equal(2, details.Portions);
        Assert.Null(details.IsFavourite);
        Assert.Equal(4, details.Ingredients.Count);
        Assert.Equal(200, details.Ingredients.Single(i => i.Name == "Spaghetti").Quantity);
        Assert.Equal("g", details.Ingredients.Single(i => i.Name == "Spaghetti").Unit);
        Assert.Equal(
            ["Looks delicious", "Family favorite", "Made it twice"],
            details.Comments.Select(c => c.Text).ToArray());
        Assert.Equal("bob", details.Comments[0].AuthorName);
    }

    [Fact]
    public async Task Authorized_user_sees_active_favourites_mark()
    {
        var fx = new RecipeFixtures();
        fx.Favourites.Add(fx.Bob, fx.PastaCarbonara.RecipeId);

        var details = await fx.Sender.Send(new GetRecipeDetailsQuery(fx.PastaCarbonara.RecipeId, fx.Bob));

        Assert.NotNull(details);
        Assert.True(details.IsFavourite);
    }

    [Fact]
    public async Task Scales_ingredients_when_portions_change()
    {
        var fx = new RecipeFixtures();

        var details = await fx.Sender.Send(
            new GetRecipeDetailsQuery(fx.PastaCarbonara.RecipeId, ViewerId: null, TargetPortions: new Portions(4)));

        Assert.NotNull(details);
        Assert.Equal(4, details.Portions);
        Assert.Equal(400, details.Ingredients.Single(i => i.Name == "Spaghetti").Quantity);
        Assert.Equal(4, details.Ingredients.Single(i => i.Name == "Eggs").Quantity);
        Assert.Equal(200, details.Ingredients.Single(i => i.Name == "Bacon").Quantity);
        Assert.Equal(100, details.Ingredients.Single(i => i.Name == "Milk").Quantity);
    }

    [Fact]
    public async Task Returns_null_when_recipe_is_not_visible()
    {
        var fx = new RecipeFixtures();

        var details = await fx.Sender.Send(new GetRecipeDetailsQuery(fx.ChocolateCake.RecipeId, ViewerId: null));

        Assert.Null(details);
    }

    [Fact]
    public async Task Returns_null_when_recipe_is_missing()
    {
        var fx = new RecipeFixtures();

        var details = await fx.Sender.Send(new GetRecipeDetailsQuery(RecipeId.New(), fx.Alice));

        Assert.Null(details);
    }
}
