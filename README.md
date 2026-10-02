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

- UI: `http://localhost:5173` (or `FRONTEND_PORT`)
- API: `http://localhost:8080` (or `API_PORT`)
- OpenAPI: `http://localhost:8080/openapi/v1.json`
- MinIO console: `http://localhost:9001`

Credentials live only in `.env` (see `.env.example`). Do not commit `.env`.

### Auth (JWT)

- `POST /api/auth/register` — body `{ "userName": "...", "password": "..." }`
- `POST /api/auth/login` — returns `{ userId, userName, accessToken }`
- Send `Authorization: Bearer <token>` on protected endpoints
- Seed users: `alice` / `bob`, password `Password1!` (local demo only)

### Endpoints

- `GET /api/recipes?search=&sort=newest|averageRatingAsc|averageRatingDesc`
- `GET /api/recipes/{id}?portions=`
- `GET /api/photos/{fileName}`
- `PUT /api/recipes/{id}/favourite` — body `{ "isActive": true|false }` (auth required)
- `PUT /api/recipes/{id}/rating` — body `{ "stars": 1-5 }` (auth required, once)
- `POST /api/recipes/{id}/comments` — body `{ "text": "..." }` (auth required)
- `DELETE /api/recipes/{id}/comments/{commentId}` — recipe author only
- `GET /api/menu-plan` / `PUT /api/menu-plan/portions`
- `PUT|DELETE /api/menu-plan/slots/{day}/{meal}`
- `GET /api/menu-plan/shopping-list` / `GET /api/menu-plan/shopping-list/export`

## Local frontend (without Docker UI)

```bash
cd src/cookbook-web
npm install
npm run dev
```

Vite proxies `/api` to `http://localhost:8080`.

## Local API (without Docker)

Prerequisites: .NET 9 SDK, PostgreSQL running locally.

```bash
dotnet user-secrets init --project src/Cookbook.Api
dotnet user-secrets set "ConnectionStrings:Cookbook" "Host=localhost;Port=5432;Database=cookbook;Username=YOUR_USER;Password=YOUR_PASSWORD" --project src/Cookbook.Api
dotnet user-secrets set "Jwt:Key" "change-me-to-a-long-random-secret-key!!" --project src/Cookbook.Api
dotnet run --project src/Cookbook.Api
```

## Tests

```bash
dotnet test Cookbook.sln
```
