using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Signatories;
using Accordly.Domain.Enums;
using MediatR;

namespace Accordly.Application.Agreements.Commands.SubmitRegisteredSignature;

/// <summary>Records a signature for an authenticated, registered signatory.</summary>
public sealed class SubmitRegisteredSignatureCommandHandler(IAgreementRepository agreementRepository) : IRequestHandler<SubmitRegisteredSignatureCommand, SignatoryResponse?>
{
    public async Task<SignatoryResponse?> Handle(SubmitRegisteredSignatureCommand request, CancellationToken cancellationToken)
    {
        var signatory = await agreementRepository.GetSignatoryAsync(request.AgreementId, request.SignatoryId, cancellationToken);
        if (signatory is null || signatory.UserId != request.RequestingUserId || signatory.SignedAt is not null)
        {
            return null;
        }

        var agreement = await agreementRepository.GetByIdAsync(request.AgreementId, cancellationToken);
        if (agreement is null)
        {
            return null;
        }

        signatory.SignatureValue = request.SignatureValue;
        signatory.SignerIp = request.IpAddress;
        signatory.SignedAt = DateTimeOffset.UtcNow;
        signatory.VersionSignedId = agreement.CurrentVersionId == Guid.Empty ? null : agreement.CurrentVersionId;

        await agreementRepository.UpdateSignatoryAsync(signatory, cancellationToken);

        var allSignatories = await agreementRepository.GetSignatoriesForAgreementAsync(agreement.Id, cancellationToken);
        if (allSignatories is not null &&
            allSignatories.Any(candidate => candidate.Role == SignatoryRole.Signer) &&
            allSignatories.Where(candidate => candidate.Role == SignatoryRole.Signer).All(candidate => candidate.SignedAt is not null) &&
            agreement.Status != AgreementStatus.Active)
        {
            if (agreement.Status == AgreementStatus.Draft)
            {
                agreement.TransitionTo(AgreementStatus.PendingSignatures);
            }

            agreement.TransitionTo(AgreementStatus.Active);
            await agreementRepository.UpdateAsync(agreement, cancellationToken);
        }

        return new SignatoryResponse(signatory.Id, signatory.Email, signatory.Role.ToString(), signatory.SignedAt);
    }
}
