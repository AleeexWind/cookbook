using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Cookbook.Domain.Favourites;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Api.Tests;

public sealed class RecipesApiTests : IClassFixture<CookbookApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly CookbookApiFactory _factory;

    public RecipesApiTests(CookbookApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Guest_sees_only_public_recipes()
    {
        var client = await _factory.CreateSeededClientAsync();

        var response = await client.GetAsync("/api/recipes");
        response.EnsureSuccessStatusCode();

        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeListJson>>(JsonOptions);
        Assert.NotNull(recipes);
        Assert.Contains(recipes, r => r.Title == "Pasta Carbonara");
        Assert.DoesNotContain(recipes, r => r.Title == "Chicken Stir Fry");
        Assert.DoesNotContain(recipes, r => r.Title == "Chocolate Cake");
        Assert.Null(recipes.First(r => r.Title == "Pasta Carbonara").IsFavourite);
    }

    [Fact]
    public async Task Alice_sees_public_and_own_private_with_favourites()
    {
        var client = await _factory.CreateSeededClientAsync();
        await _factory.AuthenticateAsAsync(client, "alice");

        var response = await client.GetAsync("/api/recipes");
        response.EnsureSuccessStatusCode();

        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeListJson>>(JsonOptions);
        Assert.NotNull(recipes);
        Assert.True(recipes.Count >= 2);
        Assert.True(recipes.Single(r => r.Title == "Pasta Carbonara").IsFavourite);
        Assert.False(recipes.Single(r => r.Title == "Chicken Stir Fry").IsFavourite);
    }

    [Fact]
    public async Task Search_by_name_returns_carbonara()
    {
        var client = await _factory.CreateSeededClientAsync();

        var response = await client.GetAsync("/api/recipes?search=Carbonara");
        response.EnsureSuccessStatusCode();

        var recipes = await response.Content.ReadFromJsonAsync<List<RecipeListJson>>(JsonOptions);
        Assert.NotNull(recipes);
        Assert.Single(recipes);
        Assert.Equal("Pasta Carbonara", recipes[0].Title);
    }

    [Fact]
    public async Task Guest_cannot_open_private_recipe()
    {
        var client = await _factory.CreateSeededClientAsync();

        var response = await client.GetAsync($"/api/recipes/{SeedIds.ChocolateCake}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Details_scale_portions_and_bob_sees_favourite()
    {
        var client = await _factory.CreateSeededClientAsync();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<CookbookDbContext>();
            db.Favourites.Add(Favourite.Activate(new UserId(SeedIds.Bob), new RecipeId(SeedIds.PastaCarbonara)));
            await db.SaveChangesAsync();
        }

        await _factory.AuthenticateAsAsync(client, "bob");
        var response = await client.GetAsync($"/api/recipes/{SeedIds.PastaCarbonara}?portions=4");
        response.EnsureSuccessStatusCode();

        var details = await response.Content.ReadFromJsonAsync<RecipeDetailsJson>(JsonOptions);
        Assert.NotNull(details);
        Assert.Equal(4, details.Portions);
        Assert.True(details.IsFavourite);
        Assert.Equal(400, details.Ingredients.Single(i => i.Name == "Spaghetti").Quantity);
        Assert.Equal(2, details.Steps.Count);
        Assert.Equal(1, details.Steps[0].Order);
        Assert.Equal("Boil the pasta.", details.Steps[0].Instruction);
        Assert.Equal(2, details.Steps[1].Order);
        Assert.Equal("Mix eggs and bacon.", details.Steps[1].Instruction);
        Assert.Equal("bob", details.Comments[0].AuthorName);
    }

    private sealed record RecipeListJson(
        Guid Id,
        string Title,
        bool? IsFavourite);

    private sealed record RecipeDetailsJson(
        int Portions,
        bool? IsFavourite,
        List<IngredientJson> Ingredients,
        List<CookingStepJson> Steps,
        List<CommentJson> Comments);

    private sealed record IngredientJson(string Name, decimal Quantity, string Unit);

    private sealed record CookingStepJson(int Order, string Instruction);

    private sealed record CommentJson(string AuthorName, string Text);
}
