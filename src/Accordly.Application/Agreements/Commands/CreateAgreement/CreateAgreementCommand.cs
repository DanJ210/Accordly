using Accordly.Contracts.Agreements;
using MediatR;

namespace Accordly.Application.Agreements.Commands.CreateAgreement;

/// <summary>Creates a new agreement.</summary>
public sealed record CreateAgreementCommand(string Title, Guid OwnerId, DateTimeOffset? ExpiresAt) : IRequest<AgreementResponse>;
