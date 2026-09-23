using System.Security.Cryptography;
using System.Text;
using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Signatories;
using MediatR;

namespace Accordly.Application.Agreements.Commands.SubmitGuestSignature;

/// <summary>Submits a guest signature using a one-time invite token.</summary>
public sealed class SubmitGuestSignatureCommandHandler : IRequestHandler<SubmitGuestSignatureCommand, SignatoryResponse?>
{
    private readonly IAgreementRepository agreementRepository;

    public SubmitGuestSignatureCommandHandler(IAgreementRepository agreementRepository)
    {
        this.agreementRepository = agreementRepository;
    }

    public async Task<SignatoryResponse?> Handle(SubmitGuestSignatureCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.Token)));
        var signatory = await agreementRepository.GetSignatoryByTokenHashAsync(tokenHash, cancellationToken);
        if (signatory is null || signatory.SignedAt is not null)
        {
            return null;
        }

        var agreement = await agreementRepository.GetByIdAsync(signatory.AgreementId, cancellationToken);
        if (agreement is null)
        {
            return null;
        }

        signatory.SignatureValue = request.SignatureValue;
        signatory.SignedAt = DateTimeOffset.UtcNow;
        signatory.SignerIp = request.IpAddress;
        signatory.VersionSignedId = agreement.CurrentVersionId == Guid.Empty ? null : agreement.CurrentVersionId;
        signatory.InviteToken = null;

        await agreementRepository.UpdateSignatoryAsync(signatory, cancellationToken);
        return new SignatoryResponse(signatory.Id, signatory.Email, signatory.Role.ToString(), signatory.SignedAt);
    }
}
