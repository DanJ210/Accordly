namespace Accordly.Contracts.Versions;

/// <summary>Request to create a version.</summary>
public sealed record CreateVersionRequest(string Body, string? ChangeNote);
/// <summary>Version API response.</summary>
public sealed record AgreementVersionResponse(Guid Id, Guid AgreementId, int VersionNumber, string Body, Guid AuthorId, string? ChangeNote, DateTimeOffset CreatedAt);
