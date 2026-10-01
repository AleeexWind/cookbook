using Cookbook.Domain.Common;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Domain.Tests;

public sealed class RecipeVisibilityTests
{
    [Fact]
    public void Public_recipe_is_visible_to_guests()
    {
        var recipe = RecipeFactory.CreateCarbonara(visibility: Visibility.Public);

        Assert.True(recipe.IsVisibleTo(viewerId: null));
    }

    [Fact]
    public void Private_recipe_is_visible_only_to_author()
    {
        var recipe = RecipeFactory.CreateCarbonara(
            authorId: RecipeFactory.Alice,
            visibility: Visibility.Private);

        Assert.False(recipe.IsVisibleTo(viewerId: null));
        Assert.False(recipe.IsVisibleTo(RecipeFactory.Bob));
        Assert.True(recipe.IsVisibleTo(RecipeFactory.Alice));
    }
}

public sealed class RecipeRatingTests
{
    [Fact]
    public void User_can_set_rating_once_and_average_updates()
    {
        var recipe = RecipeFactory.CreateCarbonara();
        recipe.AddRating(RecipeFactory.Alice, stars: 4);

        var rating = recipe.AddRating(RecipeFactory.Bob, stars: 5);

        Assert.Equal(5, rating.Stars);
        Assert.Equal(4.5m, recipe.AverageRating);
        Assert.Equal(5, recipe.GetRatingByUser(RecipeFactory.Bob)?.Stars);
    }

    [Fact]
    public void User_cannot_change_rating_after_setting_it()
    {
        var recipe = RecipeFactory.CreateCarbonara();
        recipe.AddRating(RecipeFactory.Bob, stars: 4);

        var exception = Assert.Throws<DomainException>(() => recipe.AddRating(RecipeFactory.Bob, stars: 5));

        Assert.Equal("User has already rated this recipe.", exception.Message);
        Assert.Equal(4, recipe.GetRatingByUser(RecipeFactory.Bob)?.Stars);
    }

    [Fact]
    public void Rating_stars_must_be_between_1_and_5()
    {
        var recipe = RecipeFactory.CreateCarbonara();

        Assert.Throws<DomainException>(() => recipe.AddRating(RecipeFactory.Bob, stars: 0));
        Assert.Throws<DomainException>(() => recipe.AddRating(RecipeFactory.Bob, stars: 6));
    }
}

public sealed class RecipeCommentTests
{
    [Fact]
    public void Authorized_user_can_add_comment_as_newest()
    {
        var recipe = RecipeFactory.CreateCarbonara(authorId: RecipeFactory.Alice);
        recipe.AddComment(RecipeFactory.Alice, "Family favorite", new DateTimeOffset(2026, 9, 27, 12, 0, 0, TimeSpan.Zero));

        var comment = recipe.AddComment(
            RecipeFactory.Bob,
            "Looks delicious",
            new DateTimeOffset(2026, 9, 28, 18, 0, 0, TimeSpan.Zero));

        var newest = recipe.GetCommentsNewestFirst();
        Assert.Equal(comment.Id, newest[0].Id);
        Assert.Equal("Looks delicious", newest[0].Text);
        Assert.Equal(2, recipe.CommentsCount);
    }

    [Fact]
    public void Recipe_author_can_delete_comment()
    {
        var recipe = RecipeFactory.CreateCarbonara(authorId: RecipeFactory.Alice);
        var comment = recipe.AddComment(RecipeFactory.Bob, "Looks delicious", DateTimeOffset.UtcNow);

        recipe.RemoveComment(comment.Id, RecipeFactory.Alice);

        Assert.Empty(recipe.Comments);
    }

    [Fact]
    public void Non_author_cannot_delete_comment()
    {
        var recipe = RecipeFactory.CreateCarbonara(authorId: RecipeFactory.Alice);
        var comment = recipe.AddComment(RecipeFactory.Alice, "Family favorite", DateTimeOffset.UtcNow);

        var exception = Assert.Throws<DomainException>(
            () => recipe.RemoveComment(comment.Id, RecipeFactory.Bob));

        Assert.Equal("Only the recipe author can delete comments.", exception.Message);
        Assert.Single(recipe.Comments);
    }
}

public sealed class RecipePortionsTests
{
    [Fact]
    public void Scaling_from_2_to_4_portions_doubles_quantities()
    {
        var recipe = RecipeFactory.CreateCarbonara(basePortions: new Portions(2));

        var scaled = recipe.ScaleIngredients(new Portions(4));

        Assert.Equal(400, scaled.Single(i => i.Name == "Spaghetti").Quantity);
        Assert.Equal(4, scaled.Single(i => i.Name == "Eggs").Quantity);
        Assert.Equal(200, scaled.Single(i => i.Name == "Bacon").Quantity);
        Assert.Equal(100, scaled.Single(i => i.Name == "Milk").Quantity);
    }
}
