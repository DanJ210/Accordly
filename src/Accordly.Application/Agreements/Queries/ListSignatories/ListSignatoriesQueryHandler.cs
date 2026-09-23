using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Signatories;
using MediatR;

namespace Accordly.Application.Agreements.Queries.ListSignatories;

/// <summary>Lists signatories for an agreement.</summary>
public sealed class ListSignatoriesQueryHandler : IRequestHandler<ListSignatoriesQuery, IReadOnlyList<SignatoryResponse>?>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IAgreementAuthorizationService authorizationService;

    public ListSignatoriesQueryHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService)
    {
        this.agreementRepository = agreementRepository;
        this.authorizationService = authorizationService;
    }

    public async Task<IReadOnlyList<SignatoryResponse>?> Handle(ListSignatoriesQuery request, CancellationToken cancellationToken)
    {
        if (!await authorizationService.CanReadAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return null;
        }

        var signatories = await agreementRepository.GetSignatoriesForAgreementAsync(request.AgreementId, cancellationToken);
        return signatories
            .Select(signatory => new SignatoryResponse(signatory.Id, signatory.Email, signatory.Role.ToString(), signatory.SignedAt))
            .ToList();
    }
}
