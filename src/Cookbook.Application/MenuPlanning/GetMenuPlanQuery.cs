using Cookbook.Application.Abstractions;
using Cookbook.Application.Recipes;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.MenuPlanning;

/// <summary>
/// Returns the caller's menu plan, creating an empty one when missing.
/// </summary>
/// <param name="OwnerId">Plan owner.</param>
public sealed record GetMenuPlanQuery(UserId OwnerId) : IRequest<MenuPlanDto>;

/// <summary>
/// Handles <see cref="GetMenuPlanQuery"/>.
/// </summary>
public sealed class GetMenuPlanQueryHandler(
    IMenuPlanRepository menuPlans,
    IRecipeReadStore recipes) : IRequestHandler<GetMenuPlanQuery, MenuPlanDto>
{
    /// <inheritdoc />
    public async Task<MenuPlanDto> Handle(GetMenuPlanQuery request, CancellationToken cancellationToken)
    {
        var plan = await MenuPlanAccess.GetOrCreateAsync(menuPlans, request.OwnerId, cancellationToken);
        return await MapAsync(plan, recipes, cancellationToken);
    }

    internal static async Task<MenuPlanDto> MapAsync(
        MenuPlan plan,
        IRecipeReadStore recipes,
        CancellationToken cancellationToken)
    {
        var titles = new Dictionary<RecipeId, string>();
        foreach (var recipeId in plan.GetPlacedRecipeIds().Distinct())
        {
            var recipe = await recipes.GetByIdAsync(recipeId, cancellationToken);
            if (recipe is not null)
            {
                titles[recipeId] = recipe.Title;
            }
        }

        var slots = plan.Slots
            .OrderBy(s => DayOrder(s.Day))
            .ThenBy(s => s.MealType)
            .Select(s => new MealSlotDto(
                s.Day.ToString(),
                s.MealType.ToString(),
                s.RecipeId?.Value,
                s.RecipeId is { } id && titles.TryGetValue(id, out var title) ? title : null))
            .ToList();

        return new MenuPlanDto(plan.Portions.Value, slots);
    }

    private static int DayOrder(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => 0,
        DayOfWeek.Tuesday => 1,
        DayOfWeek.Wednesday => 2,
        DayOfWeek.Thursday => 3,
        DayOfWeek.Friday => 4,
        DayOfWeek.Saturday => 5,
        DayOfWeek.Sunday => 6,
        _ => 7
    };
}
