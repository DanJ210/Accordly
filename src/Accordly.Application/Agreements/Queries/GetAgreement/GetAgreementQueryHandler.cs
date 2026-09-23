using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Agreements;
using MediatR;

namespace Accordly.Application.Agreements.Queries.GetAgreement;

/// <summary>Handles agreement lookup.</summary>
public sealed class GetAgreementQueryHandler : IRequestHandler<GetAgreementQuery, AgreementResponse?>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IAgreementAuthorizationService authorizationService;

    /// <summary>Initializes the handler.</summary>
    public GetAgreementQueryHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService)
    {
        this.agreementRepository = agreementRepository;
        this.authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<AgreementResponse?> Handle(GetAgreementQuery request, CancellationToken cancellationToken)
    {
        var agreement = await agreementRepository.GetByIdAsync(request.AgreementId, cancellationToken);
        if (agreement is null || !await authorizationService.CanReadAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return null;
        }

        return new AgreementResponse(agreement.Id, agreement.Title, agreement.Status.ToString(), agreement.OwnerId, agreement.CreatedAt, agreement.UpdatedAt, agreement.ExpiresAt);
    }
}
