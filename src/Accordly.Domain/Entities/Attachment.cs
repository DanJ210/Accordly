using Accordly.Domain.Common;

namespace Accordly.Domain.Entities;

/// <summary>File attached to an agreement version.</summary>
public sealed class Attachment : Entity
{
    /// <summary>Gets or sets the agreement identifier.</summary>
    public Guid AgreementId { get; set; }
    /// <summary>Gets or sets the pinned version identifier.</summary>
    public Guid VersionId { get; set; }
    /// <summary>Gets or sets the original file name.</summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>Gets or sets the MIME content type.</summary>
    public string ContentType { get; set; } = string.Empty;
    /// <summary>Gets or sets the object storage key.</summary>
    public string StorageKey { get; set; } = string.Empty;
    /// <summary>Gets or sets the file size.</summary>
    public long FileSizeBytes { get; set; }
    /// <summary>Gets or sets the SHA-256 hash.</summary>
    public string Sha256Hash { get; set; } = string.Empty;
    /// <summary>Gets or sets the uploader identifier.</summary>
    public Guid UploadedById { get; set; }
    /// <summary>Gets or sets the upload timestamp.</summary>
    public DateTimeOffset UploadedAt { get; set; } = DateTimeOffset.UtcNow;
}
