using Accordly.Contracts.Agreements;
using MediatR;

namespace Accordly.Application.Agreements.Queries.GetAgreement;

/// <summary>Gets an agreement visible to a user.</summary>
public sealed record GetAgreementQuery(Guid AgreementId, Guid RequestingUserId) : IRequest<AgreementResponse?>;
