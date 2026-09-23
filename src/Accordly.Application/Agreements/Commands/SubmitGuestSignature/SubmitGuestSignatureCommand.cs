using Accordly.Contracts.Signatories;
using MediatR;

namespace Accordly.Application.Agreements.Commands.SubmitGuestSignature;

/// <summary>Submits a guest signature for a delegated signatory.</summary>
public sealed record SubmitGuestSignatureCommand(string Token, string SignatureValue, string? IpAddress) : IRequest<SignatoryResponse?>;
