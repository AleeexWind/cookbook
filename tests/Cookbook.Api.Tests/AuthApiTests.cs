using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Cookbook.Infrastructure.Auth;

namespace Cookbook.Api.Tests;

public sealed class AuthApiTests : IClassFixture<CookbookApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly CookbookApiFactory _factory;

    public AuthApiTests(CookbookApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_seed_user_returns_token()
    {
        var client = await _factory.CreateSeededClientAsync();
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { userName = "alice", password = SeedCredentials.DemoPassword });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthJson>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("alice", body.UserName);
        Assert.False(string.IsNullOrWhiteSpace(body.AccessToken));
    }

    [Fact]
    public async Task Login_wrong_password_is_unauthorized()
    {
        var client = await _factory.CreateSeededClientAsync();
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { userName = "alice", password = "wrong" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_then_use_token_for_menu_plan()
    {
        var client = await _factory.CreateSeededClientAsync();
        var register = await client.PostAsJsonAsync(
            "/api/auth/register",
            new { userName = "carol", password = "Secret1!" });
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);
        var body = await register.Content.ReadFromJsonAsync<AuthJson>(JsonOptions);
        Assert.NotNull(body);

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", body.AccessToken);
        var plan = await client.GetAsync("/api/menu-plan");
        Assert.Equal(HttpStatusCode.OK, plan.StatusCode);
    }

    private sealed record AuthJson(Guid UserId, string UserName, string AccessToken);
}
