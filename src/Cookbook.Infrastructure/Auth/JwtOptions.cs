namespace Cookbook.Infrastructure.Auth;

/// <summary>
/// JWT signing and validation settings.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Gets or sets the symmetric signing key (at least 32 characters).
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; } = "cookbook";

    /// <summary>
    /// Gets or sets the token audience.
    /// </summary>
    public string Audience { get; set; } = "cookbook";

    /// <summary>
    /// Gets or sets token lifetime in minutes.
    /// </summary>
    public int ExpiresMinutes { get; set; } = 480;
}
