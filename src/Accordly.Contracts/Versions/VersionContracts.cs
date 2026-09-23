namespace Accordly.Contracts.Versions;

/// <summary>Request to create a version.</summary>
public sealed record CreateVersionRequest(string Body, string? ChangeNote);
/// <summary>Version API response.</summary>
public sealed record AgreementVersionResponse(Guid Id, Guid AgreementId, int VersionNumber, string Body, Guid AuthorId, string? ChangeNote, DateTimeOffset CreatedAt);
/// <summary>Single change in a version diff.</summary>
public sealed record VersionDiffChange(string Kind, string Field, string? PreviousValue, string? NewValue);
/// <summary>Response returned from a version diff lookup.</summary>
public sealed record VersionDiffResponse(Guid AgreementId, Guid FromVersionId, Guid ToVersionId, int FromVersionNumber, int ToVersionNumber, DateTimeOffset FromCreatedAt, DateTimeOffset ToCreatedAt, IReadOnlyList<VersionDiffChange> Changes);
