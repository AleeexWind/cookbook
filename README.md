# Cookbook

Full-stack cookbook application (C# / React). Backend uses DDD + Clean Architecture.

## Run with Docker (recommended)

Prerequisites: Docker Desktop (or Docker Engine + Compose).

```bash
docker compose up --build
```

- API: `http://localhost:8080`
- OpenAPI: `http://localhost:8080/openapi/v1.json`
- Recipes: `GET http://localhost:8080/api/recipes`

PostgreSQL is exposed on `localhost:5432` (user/password/db: `postgres` / `postgres` / `cookbook`).

### Auth stub

Optional header `X-User: alice` or `X-User: bob`. Omit the header for a guest.

### Endpoints

- `GET /api/recipes?search=&sort=newest|averageRatingAsc|averageRatingDesc`
- `GET /api/recipes/{id}?portions=`

## Local run (without Docker)

Prerequisites: .NET 9 SDK, PostgreSQL running locally.

1. Update the connection string in [`src/Cookbook.Api/appsettings.Development.json`](src/Cookbook.Api/appsettings.Development.json) if needed.
2. Run the API:

```bash
dotnet run --project src/Cookbook.Api
```

## Tests

```bash
dotnet test Cookbook.sln
```
