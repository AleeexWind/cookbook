using Cookbook.Application.Recipes;
using Cookbook.Domain.Recipes;

namespace Cookbook.Application.Tests;

public sealed class GetRecipesQueryTests
{
    [Fact]
    public async Task Guest_sees_only_public_recipes_sorted_newest_without_favourite_mark()
    {
        var fx = new RecipeFixtures();

        var result = await fx.Sender.Send(new GetRecipesQuery(ViewerId: null));

        Assert.Single(result);
        Assert.Equal("Pasta Carbonara", result[0].Title);
        Assert.Null(result[0].IsFavourite);
        Assert.Equal(4.5m, result[0].AverageRating);
        Assert.Equal(3, result[0].CommentsCount);
        Assert.DoesNotContain(result, r => r.Title == "Chicken Stir Fry");
        Assert.DoesNotContain(result, r => r.Title == "Chocolate Cake");
    }

    [Fact]
    public async Task Authorized_user_sees_public_and_own_private_with_favourites()
    {
        var fx = new RecipeFixtures();

        var result = await fx.Sender.Send(new GetRecipesQuery(fx.Alice));

        Assert.Equal(2, result.Count);
        Assert.Equal(["Pasta Carbonara", "Chicken Stir Fry"], result.Select(r => r.Title).ToArray());
        Assert.True(result.Single(r => r.Title == "Pasta Carbonara").IsFavourite);
        Assert.False(result.Single(r => r.Title == "Chicken Stir Fry").IsFavourite);
        Assert.DoesNotContain(result, r => r.Title == "Chocolate Cake");
    }

    [Fact]
    public async Task Sort_by_average_rating_descending()
    {
        var fx = new RecipeFixtures();

        var result = await fx.Sender.Send(new GetRecipesQuery(fx.Alice, Sort: RecipeSort.AverageRatingDesc));

        Assert.Equal(["Pasta Carbonara", "Chicken Stir Fry"], result.Select(r => r.Title).ToArray());
        Assert.True(result[0].AverageRating >= result[1].AverageRating);
    }

    [Fact]
    public async Task Sort_by_average_rating_ascending()
    {
        var fx = new RecipeFixtures();

        var result = await fx.Sender.Send(new GetRecipesQuery(fx.Alice, Sort: RecipeSort.AverageRatingAsc));

        Assert.Equal(["Chicken Stir Fry", "Pasta Carbonara"], result.Select(r => r.Title).ToArray());
    }

    [Fact]
    public async Task Search_by_name()
    {
        var fx = new RecipeFixtures();

        var result = await fx.Sender.Send(new GetRecipesQuery(ViewerId: null, Search: "Carbonara"));

        Assert.Single(result);
        Assert.Equal("Pasta Carbonara", result[0].Title);
    }

    [Fact]
    public async Task Search_by_tag()
    {
        var fx = new RecipeFixtures();

        var result = await fx.Sender.Send(new GetRecipesQuery(fx.Alice, Search: "vegan"));

        Assert.Single(result);
        Assert.Equal("Chicken Stir Fry", result[0].Title);
    }

    [Fact]
    public async Task Search_by_ingredients()
    {
        var fx = new RecipeFixtures();

        var result = await fx.Sender.Send(new GetRecipesQuery(fx.Alice, Search: "chicken rice"));

        Assert.Single(result);
        Assert.Equal("Chicken Stir Fry", result[0].Title);
    }

    [Fact]
    public async Task Search_does_not_show_recipes_guest_cannot_see()
    {
        var fx = new RecipeFixtures();

        var result = await fx.Sender.Send(new GetRecipesQuery(ViewerId: null, Search: "Cake"));

        Assert.Empty(result);
    }
}
