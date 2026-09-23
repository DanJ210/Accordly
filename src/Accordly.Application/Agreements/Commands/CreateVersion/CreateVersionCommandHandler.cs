using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Versions;
using Accordly.Domain.Entities;
using Accordly.Domain.Events;
using MediatR;

namespace Accordly.Application.Agreements.Commands.CreateVersion;

/// <summary>Handles creation of the next agreement version.</summary>
public sealed class CreateVersionCommandHandler : IRequestHandler<CreateVersionCommand, AgreementVersionResponse?>
{
    private readonly IAgreementRepository agreementRepository;
    private readonly IAgreementAuthorizationService authorizationService;
    private readonly IUnitOfWork unitOfWork;

    /// <summary>Initializes the handler.</summary>
    public CreateVersionCommandHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService, IUnitOfWork unitOfWork)
    {
        this.agreementRepository = agreementRepository;
        this.authorizationService = authorizationService;
        this.unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<AgreementVersionResponse?> Handle(CreateVersionCommand request, CancellationToken cancellationToken)
    {
        var agreement = await agreementRepository.GetByIdAsync(request.AgreementId, cancellationToken);
        if (agreement is null || !await authorizationService.CanMutateAsync(request.AgreementId, request.AuthorId, cancellationToken))
        {
            return null;
        }

        var versionNumber = await agreementRepository.GetNextVersionNumberAsync(agreement.Id, cancellationToken);
        var version = new AgreementVersion
        {
            AgreementId = agreement.Id,
            VersionNumber = versionNumber,
            Body = request.Body,
            AuthorId = request.AuthorId,
            ChangeNote = request.ChangeNote
        };

        await agreementRepository.AddVersionAsync(version, cancellationToken);
        agreement.CurrentVersionId = version.Id;
        agreement.UpdatedAt = DateTimeOffset.UtcNow;
        await agreementRepository.UpdateAsync(agreement, cancellationToken);

        _ = new AgreementVersionCreatedEvent(
            agreement.Id,
            version.Id,
            version.VersionNumber,
            version.AuthorId,
            version.CreatedAt);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AgreementVersionResponse(
            version.Id,
            version.AgreementId,
            version.VersionNumber,
            version.Body,
            version.AuthorId,
            version.ChangeNote,
            version.CreatedAt);
    }
}
