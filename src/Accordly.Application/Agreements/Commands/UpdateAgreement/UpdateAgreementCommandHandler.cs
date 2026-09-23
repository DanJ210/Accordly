using System.Text.Json;
using Accordly.Application.Common.Interfaces;
using Accordly.Contracts.Agreements;
using Accordly.Domain.Entities;
using Accordly.Domain.Enums;
using MediatR;

namespace Accordly.Application.Agreements.Commands.UpdateAgreement;

/// <summary>Handles agreement updates.</summary>
public sealed class UpdateAgreementCommandHandler(IAgreementRepository agreementRepository, IAgreementAuthorizationService authorizationService, IAuditEventRecorder auditEventRecorder, IUnitOfWork unitOfWork) : IRequestHandler<UpdateAgreementCommand, AgreementResponse?>
{
    /// <inheritdoc />
    public async Task<AgreementResponse?> Handle(UpdateAgreementCommand request, CancellationToken cancellationToken)
    {
        var agreement = await agreementRepository.GetByIdAsync(request.AgreementId, cancellationToken);
        if (agreement is null || !await authorizationService.CanMutateAsync(request.AgreementId, request.RequestingUserId, cancellationToken))
        {
            return null;
        }

        if (request.Title is not null)
        {
            agreement.Title = request.Title;
        }

        if (request.ExpiresAt is not null)
        {
            agreement.ExpiresAt = request.ExpiresAt;
        }

        if (request.Status is not null && Enum.TryParse<AgreementStatus>(request.Status, true, out var status) && status != agreement.Status)
        {
            agreement.TransitionTo(status);
        }

        agreement.UpdatedAt = DateTimeOffset.UtcNow;
        await agreementRepository.UpdateAsync(agreement, cancellationToken);
        await auditEventRecorder.RecordAsync(new AuditEvent
        {
            AgreementId = agreement.Id,
            ActorId = request.RequestingUserId,
            EventType = "AgreementUpdated",
            Payload = JsonSerializer.Serialize(new
            {
                agreement.Title,
                agreement.ExpiresAt,
                Status = agreement.Status.ToString()
            })
        }, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AgreementResponse(agreement.Id, agreement.Title, agreement.Status.ToString(), agreement.OwnerId, agreement.CreatedAt, agreement.UpdatedAt, agreement.ExpiresAt);
    }
}