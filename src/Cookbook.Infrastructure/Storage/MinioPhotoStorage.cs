using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Cookbook.Infrastructure.Storage;

/// <summary>
/// MinIO-backed photo storage.
/// </summary>
public sealed class MinioPhotoStorage(
    IOptions<MinioOptions> options,
    ILogger<MinioPhotoStorage> logger) : IPhotoStorage
{
    private readonly MinioOptions _options = options.Value;
    private readonly IMinioClient? _client = CreateClient(options.Value);

    /// <inheritdoc />
    public bool IsEnabled => _client is not null;

    /// <inheritdoc />
    public async Task EnsureBucketAsync(CancellationToken cancellationToken = default)
    {
        if (_client is null)
        {
            return;
        }

        var exists = await _client.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(_options.Bucket),
            cancellationToken);
        if (!exists)
        {
            await _client.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(_options.Bucket),
                cancellationToken);
            logger.LogInformation("Created MinIO bucket {Bucket}", _options.Bucket);
        }
    }

    /// <inheritdoc />
    public async Task UploadAsync(
        string objectKey,
        Stream content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (_client is null)
        {
            return;
        }

        await _client.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(_options.Bucket)
                .WithObject(objectKey)
                .WithStreamData(content)
                .WithObjectSize(content.Length)
                .WithContentType(contentType),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(Stream Stream, string ContentType)?> OpenReadAsync(
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        if (_client is null)
        {
            return null;
        }

        try
        {
            var memory = new MemoryStream();
            var stat = await _client.StatObjectAsync(
                new StatObjectArgs().WithBucket(_options.Bucket).WithObject(objectKey),
                cancellationToken);
            await _client.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(_options.Bucket)
                    .WithObject(objectKey)
                    .WithCallbackStream(stream => stream.CopyTo(memory)),
                cancellationToken);
            memory.Position = 0;
            return (memory, stat.ContentType ?? "application/octet-stream");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read photo {ObjectKey}", objectKey);
            return null;
        }
    }

    private static IMinioClient? CreateClient(MinioOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AccessKey) || string.IsNullOrWhiteSpace(options.SecretKey))
        {
            return null;
        }

        return new MinioClient()
            .WithEndpoint(options.Endpoint)
            .WithCredentials(options.AccessKey, options.SecretKey)
            .WithSSL(options.UseSsl)
            .Build();
    }
}
