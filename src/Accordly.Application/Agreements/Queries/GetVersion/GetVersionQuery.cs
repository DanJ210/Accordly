using Accordly.Contracts.Versions;
using MediatR;

namespace Accordly.Application.Agreements.Queries.GetVersion;

/// <summary>Gets a specific version for an agreement.</summary>
public sealed record GetVersionQuery(Guid AgreementId, Guid RequestingUserId, Guid VersionId) : IRequest<AgreementVersionResponse?>;
