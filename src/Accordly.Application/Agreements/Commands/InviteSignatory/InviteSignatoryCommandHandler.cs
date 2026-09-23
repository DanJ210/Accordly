using System.Security.Cryptography;
using System.Text;
using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Signatories;
using Accordly.Domain.Entities;
using Accordly.Domain.Enums;
using MediatR;

namespace Accordly.Application.Agreements.Commands.InviteSignatory;

/// <summary>Invites a signatory to an agreement.</summary>
public sealed class InviteSignatoryCommandHandler : IRequestHandler<InviteSignatoryCommand, SignatoryResponse?>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IAgreementAuthorizationService authorizationService;
    private readonly IUnitOfWork unitOfWork;

    public InviteSignatoryCommandHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService, IUnitOfWork unitOfWork)
    {
        this.agreementRepository = agreementRepository;
        this.authorizationService = authorizationService;
        this.unitOfWork = unitOfWork;
    }

    public async Task<SignatoryResponse?> Handle(InviteSignatoryCommand request, CancellationToken cancellationToken)
    {
        var agreement = await agreementRepository.GetByIdAsync(request.AgreementId, cancellationToken);
        if (agreement is null || !await authorizationService.CanMutateAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return null;
        }

        if (!Enum.TryParse<SignatoryRole>(request.Role, true, out var signatoryRole))
        {
            return null;
        }

        var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var signatory = new Signatory
        {
            AgreementId = agreement.Id,
            Email = request.Email.Trim(),
            Role = signatoryRole,
            InviteToken = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)))
        };

        await agreementRepository.AddSignatoryAsync(signatory, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SignatoryResponse(signatory.Id, signatory.Email, signatory.Role.ToString(), signatory.SignedAt);
    }
}
