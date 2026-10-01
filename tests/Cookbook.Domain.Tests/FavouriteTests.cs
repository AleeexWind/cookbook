using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;

namespace Cookbook.Domain.Tests;

public sealed class FavouriteTests
{
    [Fact]
    public void Activate_creates_active_favourite()
    {
        var recipeId = RecipeId.New();

        var favourite = Favourite.Activate(RecipeFactory.Bob, recipeId);

        Assert.True(favourite.IsActive);
        Assert.Equal(RecipeFactory.Bob, favourite.UserId);
        Assert.Equal(recipeId, favourite.RecipeId);
    }

    [Fact]
    public void Deactivate_and_reactivate_toggle_active_state()
    {
        var favourite = Favourite.Activate(RecipeFactory.Bob, RecipeId.New());

        favourite.Deactivate();
        Assert.False(favourite.IsActive);

        favourite.Activate();
        Assert.True(favourite.IsActive);
    }
}
