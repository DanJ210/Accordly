using Accordly.Application.Common.Interfaces;
using MediatR;

namespace Accordly.Application.Agreements.Commands.DeleteSignatory;

/// <summary>Deletes a signatory invited to an agreement.</summary>
public sealed class DeleteSignatoryCommandHandler : IRequestHandler<DeleteSignatoryCommand, bool>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IAgreementAuthorizationService authorizationService;
    private readonly IUnitOfWork unitOfWork;

    public DeleteSignatoryCommandHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService, IUnitOfWork unitOfWork)
    {
        this.agreementRepository = agreementRepository;
        this.authorizationService = authorizationService;
        this.unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteSignatoryCommand request, CancellationToken cancellationToken)
    {
        var signatory = await agreementRepository.GetSignatoryAsync(request.AgreementId, request.SignatoryId, cancellationToken);
        if (signatory is null || signatory.SignedAt is not null || !await authorizationService.CanMutateAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return false;
        }

        await agreementRepository.DeleteSignatoryAsync(signatory, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
