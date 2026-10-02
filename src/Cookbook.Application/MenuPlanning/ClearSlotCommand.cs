using Cookbook.Application.Abstractions;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.MenuPlanning;

/// <summary>
/// Clears a recipe from a menu plan slot.
/// </summary>
/// <param name="OwnerId">Plan owner.</param>
/// <param name="Day">Day of week.</param>
/// <param name="MealType">Meal type.</param>
public sealed record ClearSlotCommand(
    UserId OwnerId,
    DayOfWeek Day,
    MealType MealType) : IRequest<MenuPlanDto>;

/// <summary>
/// Handles <see cref="ClearSlotCommand"/>.
/// </summary>
public sealed class ClearSlotCommandHandler(
    IMenuPlanRepository menuPlans,
    IRecipeReadStore recipes) : IRequestHandler<ClearSlotCommand, MenuPlanDto>
{
    /// <inheritdoc />
    public async Task<MenuPlanDto> Handle(ClearSlotCommand request, CancellationToken cancellationToken)
    {
        var plan = await MenuPlanAccess.GetOrCreateAsync(menuPlans, request.OwnerId, cancellationToken);
        plan.RemoveFromSlot(request.Day, request.MealType);
        await menuPlans.UpdateAsync(plan, cancellationToken);
        return await GetMenuPlanQueryHandler.MapAsync(plan, recipes, cancellationToken);
    }
}
