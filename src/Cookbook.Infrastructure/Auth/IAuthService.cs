using Cookbook.Domain.Users;

namespace Cookbook.Infrastructure.Auth;

/// <summary>
/// Result of a successful authentication.
/// </summary>
/// <param name="UserId">Authenticated user id.</param>
/// <param name="UserName">User name.</param>
/// <param name="AccessToken">JWT access token.</param>
public sealed record AuthResult(Guid UserId, string UserName, string AccessToken);

/// <summary>
/// Registers and authenticates users.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    Task<AuthResult> RegisterAsync(string userName, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates an existing user.
    /// </summary>
    Task<AuthResult?> LoginAsync(string userName, string password, CancellationToken cancellationToken = default);
}
