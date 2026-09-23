using MediatR;

namespace Accordly.Application.Agreements.Commands.DeleteAgreement;

/// <summary>Deletes an agreement owned by the requesting user.</summary>
public sealed record DeleteAgreementCommand(Guid AgreementId, Guid RequestingUserId) : IRequest<bool>;