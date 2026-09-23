using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Versions;
using MediatR;

namespace Accordly.Application.Agreements.Queries.ListVersions;

/// <summary>Handles version listing.</summary>
public sealed class ListVersionsQueryHandler : IRequestHandler<ListVersionsQuery, IReadOnlyList<AgreementVersionResponse>?>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IAgreementAuthorizationService authorizationService;

    /// <summary>Initializes the handler.</summary>
    public ListVersionsQueryHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService)
    {
        this.agreementRepository = agreementRepository;
        this.authorizationService = authorizationService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AgreementVersionResponse>?> Handle(ListVersionsQuery request, CancellationToken cancellationToken)
    {
        if (!await authorizationService.CanReadAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return null;
        }

        var versions = await agreementRepository.GetVersionsForAgreementAsync(request.AgreementId, cancellationToken);
        return versions
            .Select(version => new AgreementVersionResponse(version.Id, version.AgreementId, version.VersionNumber, version.Body, version.AuthorId, version.ChangeNote, version.CreatedAt))
            .ToList();
    }
}
