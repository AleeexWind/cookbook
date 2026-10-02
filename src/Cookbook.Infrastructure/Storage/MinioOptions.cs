namespace Cookbook.Infrastructure.Storage;

/// <summary>
/// MinIO / S3-compatible object storage settings.
/// </summary>
public sealed class MinioOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Minio";

    /// <summary>
    /// Gets or sets the endpoint host:port (without scheme).
    /// </summary>
    public string Endpoint { get; set; } = "localhost:9000";

    /// <summary>
    /// Gets or sets the access key.
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the secret key.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bucket name for recipe photos.
    /// </summary>
    public string Bucket { get; set; } = "recipes";

    /// <summary>
    /// Gets or sets whether to use HTTPS.
    /// </summary>
    public bool UseSsl { get; set; }
}
