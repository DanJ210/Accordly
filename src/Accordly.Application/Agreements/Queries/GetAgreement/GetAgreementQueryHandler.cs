using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Agreements;
using MediatR;

namespace Accordly.Application.Agreements.Queries.GetAgreement;

/// <summary>Handles agreement lookup.</summary>
public sealed class GetAgreementQueryHandler : IRequestHandler<GetAgreementQuery, AgreementResponse?>
{
    private readonly IAgreementRepository agreementRepository;

    /// <summary>Initializes the handler.</summary>
    public GetAgreementQueryHandler(IAgreementRepository agreementRepository) => this.agreementRepository = agreementRepository;

    /// <inheritdoc />
    public async Task<AgreementResponse?> Handle(GetAgreementQuery request, CancellationToken cancellationToken)
    {
        var agreement = await agreementRepository.GetByIdAsync(request.AgreementId, cancellationToken);
        if (agreement is null || agreement.OwnerId != request.RequestingUserId)
        {
            return null;
        }

        return new AgreementResponse(agreement.Id, agreement.Title, agreement.Status.ToString(), agreement.OwnerId, agreement.CreatedAt, agreement.UpdatedAt, agreement.ExpiresAt);
    }
}
