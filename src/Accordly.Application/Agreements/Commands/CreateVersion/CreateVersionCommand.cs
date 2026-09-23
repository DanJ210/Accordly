using Accordly.Contracts.Versions;
using MediatR;

namespace Accordly.Application.Agreements.Commands.CreateVersion;

/// <summary>Creates a new immutable agreement version.</summary>
public sealed record CreateVersionCommand(
    Guid AgreementId,
    Guid AuthorId,
    string Body,
    string? ChangeNote) : IRequest<AgreementVersionResponse>;
