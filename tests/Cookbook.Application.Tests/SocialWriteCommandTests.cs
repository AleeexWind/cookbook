using Cookbook.Application.Exceptions;
using Cookbook.Application.Recipes;
using Cookbook.Application.Tests.Fakes;
using Cookbook.Domain.Common;
using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Application.Tests;

public sealed class SocialWriteCommandTests
{
    [Fact]
    public async Task SetFavourite_activates_and_deactivates()
    {
        var fx = CreateFixture();
        var recipe = fx.Recipes.Values.First(r => r.Visibility == Visibility.Public);

        await fx.Sender.Send(new SetFavouriteCommand(recipe.RecipeId, fx.Bob, IsActive: true));
        var active = await fx.Favourites.GetAsync(fx.Bob, recipe.RecipeId);
        Assert.NotNull(active);
        Assert.True(active.IsActive);

        await fx.Sender.Send(new SetFavouriteCommand(recipe.RecipeId, fx.Bob, IsActive: false));
        Assert.False((await fx.Favourites.GetAsync(fx.Bob, recipe.RecipeId))!.IsActive);
    }

    [Fact]
    public async Task SetRating_once_then_rejects_second()
    {
        var fx = CreateFixture();
        var recipe = Recipe.Create(
            "Soup",
            "Hot soup",
            20,
            Difficulty.Easy,
            "soup.jpg",
            new Category("lunch"),
            Visibility.Public,
            fx.Alice,
            new Portions(2),
            DateTimeOffset.UtcNow);
        await fx.RecipeRepo.AddAsync(recipe);

        var first = await fx.Sender.Send(new SetRatingCommand(recipe.RecipeId, fx.Bob, 5));
        Assert.Equal(5, first.Stars);
        Assert.Equal(5m, first.AverageRating);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => fx.Sender.Send(new SetRatingCommand(recipe.RecipeId, fx.Bob, 4)));
        Assert.Contains("already rated", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AddComment_returns_newest_comment()
    {
        var fx = CreateFixture();
        var recipe = fx.Recipes.Values.First(r => r.Title == "Pasta Carbonara");

        var comment = await fx.Sender.Send(new AddCommentCommand(recipe.RecipeId, fx.Bob, "Tasty"));
        Assert.Equal("Tasty", comment.Text);
        Assert.Equal("bob", comment.AuthorName);
        Assert.NotEqual(Guid.Empty, comment.Id);
    }

    [Fact]
    public async Task DeleteComment_author_can_delete_non_author_cannot()
    {
        var fx = CreateFixture();
        var recipe = fx.Recipes.Values.First(r => r.Title == "Pasta Carbonara");
        var comment = await fx.Sender.Send(new AddCommentCommand(recipe.RecipeId, fx.Bob, "To delete"));

        await Assert.ThrowsAsync<DomainException>(
            () => fx.Sender.Send(new DeleteCommentCommand(recipe.RecipeId, comment.Id, fx.Bob)));

        await fx.Sender.Send(new DeleteCommentCommand(recipe.RecipeId, comment.Id, fx.Alice));
        var reloaded = await fx.RecipeRepo.GetByIdAsync(recipe.RecipeId);
        Assert.DoesNotContain(reloaded!.Comments, c => c.Id == comment.Id);
    }

    [Fact]
    public async Task Writes_on_invisible_recipe_throw_not_found()
    {
        var fx = CreateFixture();
        var cake = fx.Recipes.Values.First(r => r.Title == "Chocolate Cake");

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => fx.Sender.Send(new SetFavouriteCommand(cake.RecipeId, fx.Alice, true)));
    }

    private static SocialFixture CreateFixture()
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
            ingredients: [IngredientLine.Create("Spaghetti", 200, MeasurementUnit.Grams)],
            tags: [new Tag("quick")]);
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

        var recipeRepo = new InMemoryRecipeRepository([carbonara, cake]);
        var favouriteRepo = new InMemoryFavouriteRepository();
        var users = new InMemoryUserReadStore(new Dictionary<UserId, string>
        {
            [alice] = "alice",
            [bob] = "bob"
        });

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddSingleton<IRecipeRepository>(recipeRepo);
        services.AddSingleton<IFavouriteRepository>(favouriteRepo);
        services.AddSingleton<Cookbook.Application.Abstractions.IUserReadStore>(users);
        services.AddSingleton<Cookbook.Application.Abstractions.IRecipeReadStore>(
            new InMemoryRecipeReadStore([carbonara, cake]));
        services.AddSingleton<Cookbook.Application.Abstractions.IFavouriteReadStore>(
            new InMemoryFavouriteReadStore());

        return new SocialFixture(
            alice,
            bob,
            recipeRepo,
            favouriteRepo,
            new Dictionary<RecipeId, Recipe>
            {
                [carbonara.RecipeId] = carbonara,
                [cake.RecipeId] = cake
            },
            services.BuildServiceProvider().GetRequiredService<ISender>());
    }

    private sealed record SocialFixture(
        UserId Alice,
        UserId Bob,
        InMemoryRecipeRepository RecipeRepo,
        InMemoryFavouriteRepository Favourites,
        Dictionary<RecipeId, Recipe> Recipes,
        ISender Sender);
}
