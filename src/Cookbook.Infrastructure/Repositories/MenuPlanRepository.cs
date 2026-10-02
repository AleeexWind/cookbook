using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Recipes;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IMenuPlanRepository"/>.
/// </summary>
public sealed class MenuPlanRepository(CookbookDbContext db) : IMenuPlanRepository
{
    /// <inheritdoc />
    public async Task<MenuPlan?> GetByOwnerAsync(UserId ownerId, CancellationToken cancellationToken = default)
    {
        var plans = await db.MenuPlans
            .Include(p => p.Slots)
            .ToListAsync(cancellationToken);
        return plans.FirstOrDefault(p => p.OwnerId.Equals(ownerId));
    }

    /// <inheritdoc />
    public async Task AddAsync(MenuPlan menuPlan, CancellationToken cancellationToken = default)
    {
        await db.MenuPlans.AddAsync(menuPlan, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(MenuPlan menuPlan, CancellationToken cancellationToken = default)
    {
        await db.SaveChangesAsync(cancellationToken);
    }
}
