using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Cookbook.Infrastructure.Persistence;

namespace Cookbook.Api.Tests;

public sealed class MenuPlanApiTests : IClassFixture<CookbookApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly CookbookApiFactory _factory;

    public MenuPlanApiTests(CookbookApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Guest_cannot_open_menu_plan()
    {
        var client = await _factory.CreateSeededClientAsync();
        var response = await client.GetAsync("/api/menu-plan");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Alice_gets_empty_plan_places_and_exports_csv()
    {
        var client = await _factory.CreateSeededClientAsync();
        await _factory.AuthenticateAsAsync(client, "bob");

        var planResponse = await client.GetFromJsonAsync<MenuPlanJson>("/api/menu-plan", JsonOptions);
        Assert.NotNull(planResponse);
        Assert.Equal(2, planResponse.Portions);
        Assert.Equal(21, planResponse.Slots.Count);

        var emptyList = await client.GetAsync("/api/menu-plan/shopping-list");
        Assert.Equal(HttpStatusCode.BadRequest, emptyList.StatusCode);

        var place = await client.PutAsJsonAsync(
            "/api/menu-plan/slots/Monday/Dinner",
            new { recipeId = SeedIds.PastaCarbonara });
        Assert.Equal(HttpStatusCode.OK, place.StatusCode);

        var portions = await client.PutAsJsonAsync("/api/menu-plan/portions", new { portions = 3 });
        Assert.Equal(HttpStatusCode.OK, portions.StatusCode);

        var list = await client.GetFromJsonAsync<ShoppingListJson>(
            "/api/menu-plan/shopping-list",
            JsonOptions);
        Assert.NotNull(list);
        Assert.Contains(list.Items, i => i.Ingredient == "Spaghetti" && i.Quantity == 300 && i.Unit == "g");

        var export = await client.GetAsync("/api/menu-plan/shopping-list/export");
        Assert.Equal(HttpStatusCode.OK, export.StatusCode);
        Assert.Equal("text/csv", export.Content.Headers.ContentType?.MediaType);
        var csv = await export.Content.ReadAsStringAsync();
        Assert.Contains("Ingredient,Quantity,Unit", csv);
        Assert.Contains("Spaghetti,300,g", csv);

        var clear = await client.DeleteAsync("/api/menu-plan/slots/Monday/Dinner");
        Assert.Equal(HttpStatusCode.OK, clear.StatusCode);
        var afterClear = await clear.Content.ReadFromJsonAsync<MenuPlanJson>(JsonOptions);
        Assert.Null(afterClear!.Slots.Single(s => s.Day == "Monday" && s.Meal == "Dinner").RecipeId);
    }

    private sealed record MenuPlanJson(int Portions, List<MealSlotJson> Slots);
    private sealed record MealSlotJson(string Day, string Meal, Guid? RecipeId, string? RecipeTitle);
    private sealed record ShoppingListJson(List<ShoppingListItemJson> Items);
    private sealed record ShoppingListItemJson(string Ingredient, decimal Quantity, string Unit);
}
