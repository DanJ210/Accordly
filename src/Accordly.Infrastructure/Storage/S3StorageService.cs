using Accordly.Application.Common.Interfaces;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;

namespace Accordly.Infrastructure.Storage;

/// <summary>S3-compatible object storage adapter.</summary>
public sealed class S3StorageService : IStorageService
{
    private readonly IAmazonS3 client;
    private readonly string bucket;
    public S3StorageService(IConfiguration configuration)
    {
        bucket = configuration["Storage:Bucket"] ?? "accordly";
        client = new AmazonS3Client(configuration["Storage:AccessKey"], configuration["Storage:SecretKey"], new AmazonS3Config { ServiceURL = configuration["Storage:Endpoint"], ForcePathStyle = true });
    }
    /// <inheritdoc />
    public Task UploadAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default) => client.PutObjectAsync(new PutObjectRequest { BucketName = bucket, Key = key, InputStream = content, ContentType = contentType }, cancellationToken);
    /// <inheritdoc />
    public async Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken = default) => (await client.GetObjectAsync(bucket, key, cancellationToken)).ResponseStream;
    /// <inheritdoc />
    public Task DeleteAsync(string key, CancellationToken cancellationToken = default) => client.DeleteObjectAsync(bucket, key, cancellationToken);
}
