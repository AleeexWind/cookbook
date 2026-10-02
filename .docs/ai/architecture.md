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
- Menu planning: `GetMenuPlanQuery`, `SetMenuPlanPortionsCommand`, `PlaceRecipeInSlotCommand`, `ClearSlotCommand`, `GetShoppingListQuery`
- CSV export via `ShoppingListCsvFormatter` (application adapter)
- Read ports: `IRecipeReadStore`, `IFavouriteReadStore`, `IUserReadStore`
- Write ports: `IRecipeRepository`, `IFavouriteRepository`, `IMenuPlanRepository`

## Infrastructure + API

- EF Core + PostgreSQL (`Cookbook.Infrastructure`), migrations + seed (alice/bob + 3 recipes)
- REST API (`Cookbook.Api`):
  - `GET /api/recipes`, `GET /api/recipes/{id}`
  - `PUT /api/recipes/{id}/favourite`, `PUT /api/recipes/{id}/rating`
  - `POST /api/recipes/{id}/comments`, `DELETE /api/recipes/{id}/comments/{commentId}`
  - `GET /api/menu-plan`, `PUT /api/menu-plan/portions`
  - `PUT /api/menu-plan/slots/{day}/{meal}`, `DELETE /api/menu-plan/slots/{day}/{meal}`
  - `GET /api/menu-plan/shopping-list`, `GET /api/menu-plan/shopping-list/export`
  - `POST /api/auth/login`, `POST /api/auth/register`
  - `GET /api/photos/{fileName}`
- Auth: JWT Bearer (`POST /api/auth/login|register`); seed users alice/bob with demo password
- MinIO for recipe photo placeholders (`GET /api/photos/{fileName}`); seed uploads when MinIO is configured
- Seed: 28 recipes, 50+ ingredient lines, 20+ comments, alice week menu plan
- Docker Compose: PostgreSQL + MinIO + API + React UI (`docker compose up --build`)
- Frontend: React (Russian UI) in `src/cookbook-web` — recipes, auth, menu plan, shopping list

## Next

- Pagination, dashboard stats, fuller recipe CRUD (product extras from context.md)
