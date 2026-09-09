namespace Accordly.Application.Common.Interfaces;

/// <summary>Abstraction over object storage.</summary>
public interface IStorageService
{
    /// <summary>Uploads content to storage.</summary>
    Task UploadAsync(string key, Stream content, string contentType, CancellationToken cancellationToken = default);
    /// <summary>Downloads content from storage.</summary>
    Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken = default);
    /// <summary>Deletes content from storage.</summary>
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
