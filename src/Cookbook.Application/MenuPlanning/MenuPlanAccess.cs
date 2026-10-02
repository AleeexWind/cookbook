using Cookbook.Application.Abstractions;
using Cookbook.Application.Exceptions;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;

namespace Cookbook.Application.MenuPlanning;

internal static class MenuPlanAccess
{
    public static async Task<MenuPlan> GetOrCreateAsync(
        IMenuPlanRepository menuPlans,
        UserId ownerId,
        CancellationToken cancellationToken)
    {
        var plan = await menuPlans.GetByOwnerAsync(ownerId, cancellationToken);
        if (plan is not null)
        {
            return plan;
        }

        plan = MenuPlan.CreateEmpty(ownerId);
        await menuPlans.AddAsync(plan, cancellationToken);
        return plan;
    }

    public static async Task EnsureRecipeVisibleAsync(
        IRecipeReadStore recipes,
        RecipeId recipeId,
        UserId userId,
        CancellationToken cancellationToken)
    {
        var recipe = await recipes.GetByIdAsync(recipeId, cancellationToken);
        if (recipe is null || !recipe.IsVisibleTo(userId))
        {
            throw new EntityNotFoundException("Recipe was not found.");
        }
    }
}
