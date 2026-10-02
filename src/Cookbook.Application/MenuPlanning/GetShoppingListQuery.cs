using Cookbook.Application.Abstractions;
using Cookbook.Application.Recipes;
using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using MediatR;

namespace Cookbook.Application.MenuPlanning;

/// <summary>
/// Generates a shopping list from the caller's menu plan.
/// </summary>
/// <param name="OwnerId">Plan owner.</param>
public sealed record GetShoppingListQuery(UserId OwnerId) : IRequest<ShoppingListDto>;

/// <summary>
/// Handles <see cref="GetShoppingListQuery"/>.
/// </summary>
public sealed class GetShoppingListQueryHandler(
    IMenuPlanRepository menuPlans,
    IRecipeReadStore recipes) : IRequestHandler<GetShoppingListQuery, ShoppingListDto>
{
    private readonly ShoppingListGenerator _generator = new();

    /// <inheritdoc />
    public async Task<ShoppingListDto> Handle(GetShoppingListQuery request, CancellationToken cancellationToken)
    {
        var plan = await MenuPlanAccess.GetOrCreateAsync(menuPlans, request.OwnerId, cancellationToken);
        var recipeMap = new Dictionary<RecipeId, Recipe>();
        foreach (var recipeId in plan.GetPlacedRecipeIds().Distinct())
        {
            var recipe = await recipes.GetByIdAsync(recipeId, cancellationToken);
            if (recipe is not null)
            {
                recipeMap[recipeId] = recipe;
            }
        }

        var list = _generator.Generate(plan, recipeMap);
        var items = list.Items
            .Select(i => new ShoppingListItemDto(i.Name, i.Quantity, MeasurementUnitFormatter.ToLabel(i.Unit)))
            .ToList();
        return new ShoppingListDto(items);
    }
}
