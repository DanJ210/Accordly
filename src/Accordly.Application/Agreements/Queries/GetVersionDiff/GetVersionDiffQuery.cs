using Accordly.Contracts.Versions;
using MediatR;

namespace Accordly.Application.Agreements.Queries.GetVersionDiff;

/// <summary>Gets a transport-safe diff between two agreement versions.</summary>
public sealed record GetVersionDiffQuery(Guid AgreementId, Guid RequestingUserId, Guid FromVersionId, Guid ToVersionId) : IRequest<VersionDiffResponse?>;
