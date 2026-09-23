using Accordly.Contracts.Versions;

namespace Accordly.Contracts.Agreements;

/// <summary>Request to create an agreement.</summary>
public sealed record CreateAgreementRequest(string Title, DateTimeOffset? ExpiresAt);
/// <summary>Agreement API response.</summary>
public sealed record AgreementResponse(Guid Id, string Title, string Status, Guid OwnerId, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? ExpiresAt);
/// <summary>Agreement detail response including the current immutable version.</summary>
public sealed record AgreementDetailResponse(Guid Id, string Title, string Status, Guid OwnerId, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt, DateTimeOffset? ExpiresAt, AgreementVersionResponse? CurrentVersion);
