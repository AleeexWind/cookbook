# Architecture

- Used DDD approach
- Used Clean Architecture
- Backend should use EF Core and code-first approach
- For DB use PostgreSQL and use migrations
- Frontend and backend interaction should be implemented via REST

## Domain aggregates (recipes context)

- **Recipe** — ingredients, cooking steps, ratings (immutable per user), comments (author may delete)
- **Favourite** — user ↔ recipe bookmark with active/inactive state
- **MenuPlan** — week slots (Mon–Sun × breakfast/lunch/dinner) and shared portions
- **ShoppingList** — derived via `ShoppingListGenerator` (not persisted as source of truth)

Ubiquitous language: root `CONTEXT.md`.

## Application layer

- MediatR queries for recipes list and details (`GetRecipesQuery`, `GetRecipeDetailsQuery`)
- MediatR commands: `SetFavouriteCommand`, `SetRatingCommand`, `AddCommentCommand`, `DeleteCommentCommand`
- Read ports: `IRecipeReadStore`, `IFavouriteReadStore`, `IUserReadStore`
- Write ports: `IRecipeRepository`, `IFavouriteRepository`

## Infrastructure + API

- EF Core + PostgreSQL (`Cookbook.Infrastructure`), initial migration + seed (alice/bob + 3 recipes)
- REST API (`Cookbook.Api`):
  - `GET /api/recipes`, `GET /api/recipes/{id}`
  - `PUT /api/recipes/{id}/favourite`, `PUT /api/recipes/{id}/rating`
  - `POST /api/recipes/{id}/comments`, `DELETE /api/recipes/{id}/comments/{commentId}`
- Auth stub: optional `X-User: alice|bob` header (JWT later); writes require auth
- Docker Compose: PostgreSQL + API (`docker compose up --build`); DB credentials via gitignored `.env` (see `.env.example`)

## Next

- JWT auth (replace `X-User` stub)
- React frontend
- Menu plan + shopping list APIs
