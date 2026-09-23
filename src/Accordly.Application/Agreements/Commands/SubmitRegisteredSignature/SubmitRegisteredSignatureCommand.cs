using Accordly.Contracts.Signatories;
using MediatR;

namespace Accordly.Application.Agreements.Commands.SubmitRegisteredSignature;

/// <summary>Submits a registered signatory's signature for the current agreement version.</summary>
public sealed record SubmitRegisteredSignatureCommand(
    Guid AgreementId,
    Guid SignatoryId,
    Guid RequestingUserId,
    string SignatureValue,
    string? IpAddress) : IRequest<SignatoryResponse?>;
