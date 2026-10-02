using Cookbook.Domain.MenuPlanning;
using Cookbook.Domain.Users;

namespace Cookbook.Application.Tests.Fakes;

internal sealed class InMemoryMenuPlanRepository : IMenuPlanRepository
{
    private readonly List<MenuPlan> _plans = [];

    public Task<MenuPlan?> GetByOwnerAsync(UserId ownerId, CancellationToken cancellationToken = default)
        => Task.FromResult(_plans.FirstOrDefault(p => p.OwnerId.Equals(ownerId)));

    public Task AddAsync(MenuPlan menuPlan, CancellationToken cancellationToken = default)
    {
        _plans.Add(menuPlan);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(MenuPlan menuPlan, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
