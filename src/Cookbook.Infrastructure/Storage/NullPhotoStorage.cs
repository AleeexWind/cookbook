namespace Cookbook.Infrastructure.Storage;

/// <summary>
/// No-op photo storage used when MinIO is not configured (e.g. tests).
/// </summary>
public sealed class NullPhotoStorage : IPhotoStorage
{
    /// <inheritdoc />
    public bool IsEnabled => false;

    /// <inheritdoc />
    public Task EnsureBucketAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc />
    public Task UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <inheritdoc />
    public Task<(Stream Stream, string ContentType)?> OpenReadAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
        => Task.FromResult<(Stream Stream, string ContentType)?>(null);
}
