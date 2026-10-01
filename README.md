# Cookbook

Full-stack cookbook application (C# / React). Backend uses DDD + Clean Architecture.

## Run with Docker (recommended)

Prerequisites: Docker Desktop (or Docker Engine + Compose).

1. Copy env template and edit if needed (`.env` is gitignored):

```bash
cp .env.example .env
```

2. Start the stack:

```bash
docker compose up --build
```

- API: `http://localhost:8080` (or `API_PORT` from `.env`)
- OpenAPI: `http://localhost:8080/openapi/v1.json`
- Recipes: `GET http://localhost:8080/api/recipes`

Credentials live only in `.env` (see `.env.example`). Do not commit `.env`.

### Auth stub

Optional header `X-User: alice` or `X-User: bob`. Omit the header for a guest.

### Endpoints

- `GET /api/recipes?search=&sort=newest|averageRatingAsc|averageRatingDesc`
- `GET /api/recipes/{id}?portions=`
- `PUT /api/recipes/{id}/favourite` — body `{ "isActive": true|false }` (auth required)
- `PUT /api/recipes/{id}/rating` — body `{ "stars": 1-5 }` (auth required, once)
- `POST /api/recipes/{id}/comments` — body `{ "text": "..." }` (auth required)
- `DELETE /api/recipes/{id}/comments/{commentId}` — recipe author only

## Local run (without Docker)

Prerequisites: .NET 9 SDK, PostgreSQL running locally.

Set the connection string via user secrets or environment (do not put passwords in committed appsettings):

```bash
dotnet user-secrets init --project src/Cookbook.Api
dotnet user-secrets set "ConnectionStrings:Cookbook" "Host=localhost;Port=5432;Database=cookbook;Username=YOUR_USER;Password=YOUR_PASSWORD" --project src/Cookbook.Api
dotnet run --project src/Cookbook.Api
```

## Tests

```bash
dotnet test Cookbook.sln
```
