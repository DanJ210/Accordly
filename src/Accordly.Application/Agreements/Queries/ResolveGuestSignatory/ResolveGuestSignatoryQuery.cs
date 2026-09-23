using Accordly.Contracts.Signatories;
using MediatR;

namespace Accordly.Application.Agreements.Queries.ResolveGuestSignatory;

/// <summary>Resolves a guest signatory by invitation token.</summary>
public sealed record ResolveGuestSignatoryQuery(string Token) : IRequest<SignatoryResponse?>;
