using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Api.Tests;

public sealed class SocialWriteApiTests : IClassFixture<CookbookApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly CookbookApiFactory _factory;

    public SocialWriteApiTests(CookbookApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Guest_cannot_set_favourite()
    {
        var client = await _factory.CreateSeededClientAsync();
        var response = await client.PutAsJsonAsync(
            $"/api/recipes/{SeedIds.PastaCarbonara}/favourite",
            new { isActive = true });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Bob_can_activate_and_deactivate_favourite()
    {
        var client = await _factory.CreateSeededClientAsync();
        client.DefaultRequestHeaders.Add("X-User", "bob");

        var activate = await client.PutAsJsonAsync(
            $"/api/recipes/{SeedIds.PastaCarbonara}/favourite",
            new { isActive = true });
        Assert.Equal(HttpStatusCode.NoContent, activate.StatusCode);

        var details = await client.GetFromJsonAsync<RecipeDetailsJson>(
            $"/api/recipes/{SeedIds.PastaCarbonara}",
            JsonOptions);
        Assert.True(details!.IsFavourite);

        var deactivate = await client.PutAsJsonAsync(
            $"/api/recipes/{SeedIds.PastaCarbonara}/favourite",
            new { isActive = false });
        Assert.Equal(HttpStatusCode.NoContent, deactivate.StatusCode);

        details = await client.GetFromJsonAsync<RecipeDetailsJson>(
            $"/api/recipes/{SeedIds.PastaCarbonara}",
            JsonOptions);
        Assert.False(details!.IsFavourite);
    }

    [Fact]
    public async Task Bob_can_rate_once_then_conflict()
    {
        var client = await _factory.CreateSeededClientAsync();
        var recipeId = await AddPublicRecipeAsync();

        client.DefaultRequestHeaders.Add("X-User", "bob");
        var first = await client.PutAsJsonAsync($"/api/recipes/{recipeId}/rating", new { stars = 5 });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await client.PutAsJsonAsync($"/api/recipes/{recipeId}/rating", new { stars = 4 });
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Bob_comments_and_alice_deletes_bob_cannot()
    {
        var client = await _factory.CreateSeededClientAsync();
        client.DefaultRequestHeaders.Add("X-User", "bob");

        var create = await client.PostAsJsonAsync(
            $"/api/recipes/{SeedIds.PastaCarbonara}/comments",
            new { text = "Looks delicious again" });
        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var comment = await create.Content.ReadFromJsonAsync<CommentJson>(JsonOptions);
        Assert.NotNull(comment);

        var bobDelete = await client.DeleteAsync(
            $"/api/recipes/{SeedIds.PastaCarbonara}/comments/{comment.Id}");
        Assert.Equal(HttpStatusCode.Forbidden, bobDelete.StatusCode);

        client.DefaultRequestHeaders.Remove("X-User");
        client.DefaultRequestHeaders.Add("X-User", "alice");
        var aliceDelete = await client.DeleteAsync(
            $"/api/recipes/{SeedIds.PastaCarbonara}/comments/{comment.Id}");
        Assert.Equal(HttpStatusCode.NoContent, aliceDelete.StatusCode);
    }

    private async Task<Guid> AddPublicRecipeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CookbookDbContext>();
        var recipe = Recipe.Create(
            "Test Salad",
            "Fresh salad",
            10,
            Difficulty.Easy,
            "salad.jpg",
            new Category("lunch"),
            Visibility.Public,
            new UserId(SeedIds.Alice),
            new Portions(2),
            DateTimeOffset.UtcNow,
            id: Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"));
        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();
        return recipe.Id;
    }

    private sealed record RecipeDetailsJson(bool? IsFavourite);

    private sealed record CommentJson(Guid Id, string AuthorName, string Text);
}
