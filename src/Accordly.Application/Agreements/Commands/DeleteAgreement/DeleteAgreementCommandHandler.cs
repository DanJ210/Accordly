using Accordly.Application.Common.Interfaces;
using MediatR;

namespace Accordly.Application.Agreements.Commands.DeleteAgreement;

/// <summary>Handles agreement deletion.</summary>
public sealed class DeleteAgreementCommandHandler(IAgreementRepository agreementRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteAgreementCommand, bool>
{
    /// <inheritdoc />
    public async Task<bool> Handle(DeleteAgreementCommand request, CancellationToken cancellationToken)
    {
        var agreement = await agreementRepository.GetByIdAsync(request.AgreementId, cancellationToken);
        if (agreement is null || agreement.OwnerId != request.RequestingUserId)
        {
            return false;
        }

        await agreementRepository.DeleteAsync(agreement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}