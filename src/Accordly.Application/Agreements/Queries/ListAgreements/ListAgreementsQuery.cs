using Accordly.Contracts.Agreements;
using MediatR;

namespace Accordly.Application.Agreements.Queries.ListAgreements;

/// <summary>Lists agreements visible to the requesting user.</summary>
public sealed record ListAgreementsQuery(Guid RequestingUserId) : IRequest<IReadOnlyList<AgreementResponse>>;