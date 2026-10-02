namespace Cookbook.Infrastructure.Storage;

/// <summary>
/// Stores and retrieves recipe photo objects.
/// </summary>
public interface IPhotoStorage
{
    /// <summary>
    /// Gets whether storage is configured and usable.
    /// </summary>
    bool IsEnabled { get; }

    /// <summary>
    /// Ensures the photo bucket exists.
    /// </summary>
    Task EnsureBucketAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads photo bytes under the given object key.
    /// </summary>
    Task UploadAsync(string objectKey, Stream content, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a read stream for an object, or null when missing.
    /// </summary>
    Task<(Stream Stream, string ContentType)?> OpenReadAsync(string objectKey, CancellationToken cancellationToken = default);
}
