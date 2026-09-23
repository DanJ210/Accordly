using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Versions;
using MediatR;

namespace Accordly.Application.Agreements.Queries.GetVersionDiff;

/// <summary>Handles version-diff calculations.</summary>
public sealed class GetVersionDiffQueryHandler : IRequestHandler<GetVersionDiffQuery, VersionDiffResponse?>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IAgreementAuthorizationService authorizationService;

    public GetVersionDiffQueryHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService)
    {
        this.agreementRepository = agreementRepository;
        this.authorizationService = authorizationService;
    }

    public async Task<VersionDiffResponse?> Handle(GetVersionDiffQuery request, CancellationToken cancellationToken)
    {
        if (!await authorizationService.CanReadAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return null;
        }

        var fromVersion = await agreementRepository.GetVersionAsync(request.AgreementId, request.FromVersionId, cancellationToken);
        var toVersion = await agreementRepository.GetVersionAsync(request.AgreementId, request.ToVersionId, cancellationToken);

        if (fromVersion is null || toVersion is null || fromVersion.AgreementId != request.AgreementId || toVersion.AgreementId != request.AgreementId)
        {
            return null;
        }

        var changes = new List<VersionDiffChange>
        {
            new VersionDiffChange("added", "body", null, toVersion.Body),
            new VersionDiffChange("removed", "body", fromVersion.Body, null),
            new VersionDiffChange("unchanged", "body", fromVersion.Body, toVersion.Body)
        };

        return new VersionDiffResponse(request.AgreementId, fromVersion.Id, toVersion.Id, fromVersion.VersionNumber, toVersion.VersionNumber, fromVersion.CreatedAt, toVersion.CreatedAt, changes);
    }
}
