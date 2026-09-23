using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Agreements;
using Accordly.Contracts.Versions;
using MediatR;

namespace Accordly.Application.Agreements.Queries.GetAgreement;

/// <summary>Handles agreement lookup.</summary>
public sealed class GetAgreementQueryHandler : IRequestHandler<GetAgreementQuery, AgreementDetailResponse?>
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
    public async Task<AgreementDetailResponse?> Handle(GetAgreementQuery request, CancellationToken cancellationToken)
    {
        var agreement = await agreementRepository.GetByIdAsync(request.AgreementId, cancellationToken);
        if (agreement is null || !await authorizationService.CanReadAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return null;
        }

        var currentVersion = agreement.CurrentVersionId == Guid.Empty
            ? null
            : await agreementRepository.GetVersionAsync(agreement.Id, agreement.CurrentVersionId, cancellationToken);
        var versionResponse = currentVersion is null
            ? null
            : new AgreementVersionResponse(currentVersion.Id, currentVersion.AgreementId, currentVersion.VersionNumber, currentVersion.Body, currentVersion.AuthorId, currentVersion.ChangeNote, currentVersion.CreatedAt);

        return new AgreementDetailResponse(agreement.Id, agreement.Title, agreement.Status.ToString(), agreement.OwnerId, agreement.CreatedAt, agreement.UpdatedAt, agreement.ExpiresAt, versionResponse);
    }
}
