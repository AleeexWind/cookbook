using Cookbook.Domain.Common;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.MenuPlanning;

/// <summary>
/// Sets the shared portions on the caller's menu plan.
/// </summary>
/// <param name="OwnerId">Plan owner.</param>
/// <param name="Portions">New portions value.</param>
public sealed record SetMenuPlanPortionsCommand(UserId OwnerId, int Portions) : IRequest<MenuPlanDto>;

/// <summary>
/// Handles <see cref="SetMenuPlanPortionsCommand"/>.
/// </summary>
public sealed class SetMenuPlanPortionsCommandHandler(
    IMenuPlanRepository menuPlans,
    Cookbook.Application.Abstractions.IRecipeReadStore recipes)
    : IRequestHandler<SetMenuPlanPortionsCommand, MenuPlanDto>
{
    /// <inheritdoc />
    public async Task<MenuPlanDto> Handle(SetMenuPlanPortionsCommand request, CancellationToken cancellationToken)
    {
        var plan = await MenuPlanAccess.GetOrCreateAsync(menuPlans, request.OwnerId, cancellationToken);
        plan.SetPortions(new Portions(request.Portions));
        await menuPlans.UpdateAsync(plan, cancellationToken);
        return await GetMenuPlanQueryHandler.MapAsync(plan, recipes, cancellationToken);
    }
}
