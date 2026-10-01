using Cookbook.Application.Abstractions;
using Cookbook.Domain.Users;

namespace Cookbook.Application.Tests.Fakes;

internal sealed class InMemoryUserReadStore : IUserReadStore
{
    private readonly Dictionary<UserId, string> _names;

    public InMemoryUserReadStore(IDictionary<UserId, string> names)
    {
        _names = new Dictionary<UserId, string>(names);
    }

    public Task<string?> GetDisplayNameAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        _names.TryGetValue(userId, out var name);
        return Task.FromResult(name);
    }
}
