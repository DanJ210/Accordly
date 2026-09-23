using Accordly.Contracts.Signatories;
using MediatR;

namespace Accordly.Application.Agreements.Commands.InviteSignatory;

/// <summary>Invites a signatory to an agreement.</summary>
public sealed record InviteSignatoryCommand(Guid AgreementId, Guid RequestingUserId, string Email, string Role) : IRequest<SignatoryResponse?>;
