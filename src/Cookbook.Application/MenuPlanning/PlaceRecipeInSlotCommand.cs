using Cookbook.Application.Abstractions;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.MenuPlanning;

/// <summary>
/// Places a recipe into a menu plan slot.
/// </summary>
/// <param name="OwnerId">Plan owner.</param>
/// <param name="Day">Day of week.</param>
/// <param name="MealType">Meal type.</param>
/// <param name="RecipeId">Recipe to place.</param>
public sealed record PlaceRecipeInSlotCommand(
    UserId OwnerId,
    DayOfWeek Day,
    MealType MealType,
    RecipeId RecipeId) : IRequest<MenuPlanDto>;

/// <summary>
/// Handles <see cref="PlaceRecipeInSlotCommand"/>.
/// </summary>
public sealed class PlaceRecipeInSlotCommandHandler(
    IMenuPlanRepository menuPlans,
    IRecipeReadStore recipes) : IRequestHandler<PlaceRecipeInSlotCommand, MenuPlanDto>
{
    /// <inheritdoc />
    public async Task<MenuPlanDto> Handle(PlaceRecipeInSlotCommand request, CancellationToken cancellationToken)
    {
        await MenuPlanAccess.EnsureRecipeVisibleAsync(
            recipes,
            request.RecipeId,
            request.OwnerId,
            cancellationToken);

        var plan = await MenuPlanAccess.GetOrCreateAsync(menuPlans, request.OwnerId, cancellationToken);
        plan.PlaceRecipe(request.Day, request.MealType, request.RecipeId);
        await menuPlans.UpdateAsync(plan, cancellationToken);
        return await GetMenuPlanQueryHandler.MapAsync(plan, recipes, cancellationToken);
    }
}
