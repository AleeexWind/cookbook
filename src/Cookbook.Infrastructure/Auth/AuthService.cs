using Cookbook.Domain.Common;
using Cookbook.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cookbook.Infrastructure.Auth;

/// <summary>
/// EF-backed authentication service.
/// </summary>
public sealed class AuthService(
    CookbookDbContext db,
    IPasswordHasher<UserAccount> passwordHasher,
    JwtTokenService tokens) : IAuthService
{
    /// <inheritdoc />
    public async Task<AuthResult> RegisterAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeUserName(userName);
        ValidatePassword(password);

        var exists = await db.Users.AnyAsync(
            u => u.UserName.ToLower() == normalized.ToLower(),
            cancellationToken);
        if (exists)
        {
            throw new DomainException("User name is already taken.");
        }

        var user = new UserAccount
        {
            Id = Guid.NewGuid(),
            UserName = normalized
        };
        user.PasswordHash = passwordHasher.HashPassword(user, password);

        await db.Users.AddAsync(user, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return new AuthResult(user.Id, user.UserName, tokens.CreateToken(user));
    }

    /// <inheritdoc />
    public async Task<AuthResult?> LoginAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeUserName(userName);
        var user = await db.Users.FirstOrDefaultAsync(
            u => u.UserName.ToLower() == normalized.ToLower(),
            cancellationToken);
        if (user is null || string.IsNullOrEmpty(user.PasswordHash))
        {
            return null;
        }

        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return new AuthResult(user.Id, user.UserName, tokens.CreateToken(user));
    }

    private static string NormalizeUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new DomainException("User name is required.");
        }

        var trimmed = userName.Trim();
        if (trimmed.Length is < 3 or > 100)
        {
            throw new DomainException("User name must be between 3 and 100 characters.");
        }

        return trimmed;
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new DomainException("Password must be at least 6 characters.");
        }
    }
}
