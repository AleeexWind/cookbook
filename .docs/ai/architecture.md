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
- Read ports: `IRecipeReadStore`, `IFavouriteReadStore`, `IUserReadStore` (implemented later in Infrastructure)
- No EF/API in this slice yet — next step is Infrastructure + REST
