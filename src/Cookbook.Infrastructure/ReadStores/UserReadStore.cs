using Cookbook.Application.Abstractions;
using Cookbook.Domain.Users;
using Cookbook.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.ReadStores;

/// <summary>
/// EF Core implementation of <see cref="IUserReadStore"/>.
/// </summary>
public sealed class UserReadStore(CookbookDbContext db) : IUserReadStore
{
    /// <inheritdoc />
    public async Task<string?> GetDisplayNameAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId.Value)
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Resolves a user id by user name (case-insensitive).
    /// </summary>
    public async Task<UserId?> FindUserIdByNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        var id = await db.Users
            .AsNoTracking()
            .Where(u => u.UserName.ToLower() == userName.ToLower())
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return id is null ? null : new UserId(id.Value);
    }
}
