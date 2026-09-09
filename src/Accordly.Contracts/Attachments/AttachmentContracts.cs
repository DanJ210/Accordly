namespace Accordly.Contracts.Attachments;

/// <summary>Attachment API response.</summary>
public sealed record AttachmentResponse(Guid Id, string FileName, string ContentType, long FileSizeBytes, string Sha256Hash, DateTimeOffset UploadedAt);
