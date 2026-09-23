using Accordly.Contracts.Signatories;
using MediatR;

namespace Accordly.Application.Agreements.Queries.ListSignatories;

/// <summary>Lists signatories for an agreement.</summary>
public sealed record ListSignatoriesQuery(Guid AgreementId, Guid RequestingUserId) : IRequest<IReadOnlyList<SignatoryResponse>?>;
