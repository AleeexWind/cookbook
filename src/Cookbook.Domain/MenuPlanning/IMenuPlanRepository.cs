using Cookbook.Domain.Users;

namespace Cookbook.Domain.MenuPlanning;

/// <summary>
/// Persistence port for menu plans.
/// </summary>
public interface IMenuPlanRepository
{
    /// <summary>
    /// Gets the menu plan for a user.
    /// </summary>
    /// <param name="ownerId">Plan owner.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The menu plan, or null when not found.</returns>
    Task<MenuPlan?> GetByOwnerAsync(UserId ownerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a menu plan.
    /// </summary>
    /// <param name="menuPlan">Plan to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(MenuPlan menuPlan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a menu plan.
    /// </summary>
    /// <param name="menuPlan">Plan to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(MenuPlan menuPlan, CancellationToken cancellationToken = default);
}
