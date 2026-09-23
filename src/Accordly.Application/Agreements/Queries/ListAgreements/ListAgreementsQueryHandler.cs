using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Agreements;
using MediatR;

namespace Accordly.Application.Agreements.Queries.ListAgreements;

/// <summary>Handles visible agreement listing.</summary>
public sealed class ListAgreementsQueryHandler(IAgreementRepository agreementRepository) : IRequestHandler<ListAgreementsQuery, IReadOnlyList<AgreementResponse>>
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<AgreementResponse>> Handle(ListAgreementsQuery request, CancellationToken cancellationToken)
    {
        var agreements = await agreementRepository.GetAllForUserAsync(request.RequestingUserId, cancellationToken);
        return agreements
            .Select(agreement => new AgreementResponse(agreement.Id, agreement.Title, agreement.Status.ToString(), agreement.OwnerId, agreement.CreatedAt, agreement.UpdatedAt, agreement.ExpiresAt))
            .ToList();
    }
}