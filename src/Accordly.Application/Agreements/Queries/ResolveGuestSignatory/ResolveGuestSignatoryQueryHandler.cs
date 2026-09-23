using System.Security.Cryptography;
using System.Text;
using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Signatories;
using MediatR;

namespace Accordly.Application.Agreements.Queries.ResolveGuestSignatory;

/// <summary>Resolves a guest signatory by hashed token.</summary>
public sealed class ResolveGuestSignatoryQueryHandler : IRequestHandler<ResolveGuestSignatoryQuery, SignatoryResponse?>
{
    private readonly IAgreementRepository agreementRepository;

    public ResolveGuestSignatoryQueryHandler(IAgreementRepository agreementRepository)
    {
        this.agreementRepository = agreementRepository;
    }

    public async Task<SignatoryResponse?> Handle(ResolveGuestSignatoryQuery request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Token)));
        var signatory = await agreementRepository.GetSignatoryByTokenHashAsync(tokenHash, cancellationToken);
        if (signatory is null || signatory.SignedAt is not null)
        {
            return null;
        }

        return new SignatoryResponse(signatory.Id, signatory.Email, signatory.Role.ToString(), signatory.SignedAt);
    }
}
