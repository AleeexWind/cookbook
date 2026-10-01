# Cookbook

Full-stack cookbook application (C# / React). Backend uses DDD + Clean Architecture.

## Backend (current)

Prerequisites: .NET 9 SDK, PostgreSQL running locally.

1. Update the connection string in [`src/Cookbook.Api/appsettings.Development.json`](src/Cookbook.Api/appsettings.Development.json) if needed.
2. Run the API:

```bash
dotnet run --project src/Cookbook.Api
```

3. Open OpenAPI at `/openapi/v1.json` (Development).

### Auth stub

Optional header `X-User: alice` or `X-User: bob`. Omit the header for a guest.

### Endpoints

- `GET /api/recipes?search=&sort=newest|averageRatingAsc|averageRatingDesc`
- `GET /api/recipes/{id}?portions=`

### Tests

```bash
dotnet test Cookbook.sln
```
