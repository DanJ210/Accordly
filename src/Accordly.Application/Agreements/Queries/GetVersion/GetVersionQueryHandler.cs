using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Versions;
using MediatR;

namespace Accordly.Application.Agreements.Queries.GetVersion;

/// <summary>Handles version lookup.</summary>
public sealed class GetVersionQueryHandler : IRequestHandler<GetVersionQuery, AgreementVersionResponse?>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IAgreementAuthorizationService authorizationService;

    /// <summary>Initializes the handler.</summary>
    public GetVersionQueryHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService)
    {
        this.agreementRepository = agreementRepository;
        this.authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<AgreementVersionResponse?> Handle(GetVersionQuery request, CancellationToken cancellationToken)
    {
        if (!await authorizationService.CanReadAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return null;
        }

        var version = await agreementRepository.GetVersionAsync(request.AgreementId, request.VersionId, cancellationToken);
        return version is null
            ? null
            : new AgreementVersionResponse(version.Id, version.AgreementId, version.VersionNumber, version.Body, version.AuthorId, version.ChangeNote, version.CreatedAt);
    }
}
