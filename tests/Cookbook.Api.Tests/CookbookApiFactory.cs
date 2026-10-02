using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Cookbook.Infrastructure;
using Cookbook.Infrastructure.Auth;
using Cookbook.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cookbook.Api.Tests;

public sealed class CookbookApiFactory : WebApplicationFactory<Program>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            services.AddInfrastructureForTests(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }

        base.Dispose(disposing);
    }

    public async Task<HttpClient> CreateSeededClientAsync()
    {
        var client = CreateClient();
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CookbookDbContext>();
        await db.Database.EnsureDeletedAsync();
        await CookbookDbSeeder.InitializeAsync(db, migrate: false);
        return client;
    }

    public async Task AuthenticateAsAsync(HttpClient client, string userName)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new { userName, password = SeedCredentials.DemoPassword });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<AuthJson>(JsonOptions);
        Assert.NotNull(body);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.AccessToken);
    }

    private sealed record AuthJson(Guid UserId, string UserName, string AccessToken);
}
