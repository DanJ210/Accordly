using Accordly.Contracts.Agreements;
using MediatR;

namespace Accordly.Application.Agreements.Commands.UpdateAgreement;

/// <summary>Updates agreement metadata and lifecycle status.</summary>
public sealed record UpdateAgreementCommand(Guid AgreementId, Guid RequestingUserId, string? Title, DateTimeOffset? ExpiresAt, string? Status) : IRequest<AgreementResponse?>;