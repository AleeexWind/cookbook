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
- Read ports: `IRecipeReadStore`, `IFavouriteReadStore`, `IUserReadStore`

## Infrastructure + API

- EF Core + PostgreSQL (`Cookbook.Infrastructure`), initial migration + seed (alice/bob + 3 recipes)
- REST API (`Cookbook.Api`): `GET /api/recipes`, `GET /api/recipes/{id}`
- Auth stub: optional `X-User: alice|bob` header (JWT later)
- Next: Docker Compose for PostgreSQL + API
