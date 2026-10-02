using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cookbook.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Cookbook.Infrastructure.Auth;

/// <summary>
/// Issues JWT access tokens for authenticated users.
/// </summary>
public sealed class JwtTokenService(IOptions<JwtOptions> options)
{
    /// <summary>
    /// Creates a signed access token for the user.
    /// </summary>
    /// <param name="user">Authenticated user.</param>
    /// <returns>JWT string.</returns>
    public string CreateToken(UserAccount user)
    {
        var jwt = options.Value;
        if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be configured and at least 32 characters.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(jwt.ExpiresMinutes);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
