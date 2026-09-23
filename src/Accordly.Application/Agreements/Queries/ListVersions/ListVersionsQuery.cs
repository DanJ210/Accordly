using Accordly.Contracts.Versions;
using MediatR;

namespace Accordly.Application.Agreements.Queries.ListVersions;

/// <summary>Lists versions for an agreement visible to a user.</summary>
public sealed record ListVersionsQuery(Guid AgreementId, Guid RequestingUserId) : IRequest<IReadOnlyList<AgreementVersionResponse>?>;
