using Cookbook.Domain.Users;

namespace Cookbook.Infrastructure.Persistence;

/// <summary>
/// Persisted user identity used for display names and authentication.
/// </summary>
public sealed class UserAccount
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the login/display name (alice, bob).
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ASP.NET Identity password hash.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Converts to a domain user id.
    /// </summary>
    public UserId ToUserId() => new(Id);
}
