using MediatR;

namespace Accordly.Application.Agreements.Commands.DeleteSignatory;

/// <summary>Deletes a signatory invitation from an agreement.</summary>
public sealed record DeleteSignatoryCommand(Guid AgreementId, Guid SignatoryId, Guid RequestingUserId) : IRequest<bool>;
