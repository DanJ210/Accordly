using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Agreements;
using Accordly.Domain.Entities;
using MediatR;

namespace Accordly.Application.Agreements.Commands.CreateAgreement;

/// <summary>Handles agreement creation.</summary>
public sealed class CreateAgreementCommandHandler : IRequestHandler<CreateAgreementCommand, AgreementResponse>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>Initializes the handler.</summary>
    public CreateAgreementCommandHandler(IAgreementRepository agreementRepository, IUnitOfWork unitOfWork)
    {
        this.agreementRepository = agreementRepository;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<AgreementResponse> Handle(CreateAgreementCommand request, CancellationToken cancellationToken)
    {
        var agreement = new Agreement
        {
            Title = request.Title,
            OwnerId = request.OwnerId,
            ExpiresAt = request.ExpiresAt
        };

        await agreementRepository.AddAsync(agreement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AgreementResponse(agreement.Id, agreement.Title, agreement.Status.ToString(), agreement.OwnerId, agreement.CreatedAt, agreement.UpdatedAt, agreement.ExpiresAt);
    }
}
