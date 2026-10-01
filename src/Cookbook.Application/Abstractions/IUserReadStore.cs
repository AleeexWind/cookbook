using Cookbook.Domain.Users;

namespace Cookbook.Application.Abstractions;

/// <summary>
/// Read port for user display names.
/// </summary>
public interface IUserReadStore
{
    /// <summary>
    /// Gets the display name for a user.
    /// </summary>
    /// <param name="userId">User id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Display name, or null when the user is unknown.</returns>
    Task<string?> GetDisplayNameAsync(UserId userId, CancellationToken cancellationToken = default);
}
