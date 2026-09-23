using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Accordly.Infrastructure.Persistence;

/// <summary>Enforces agreement membership access rules.</summary>
public sealed class AgreementAuthorizationService(AccordlyDbContext db) : IAgreementAuthorizationService
{
    /// <inheritdoc />
    public Task<bool> CanReadAsync(Guid agreementId, Guid userId, CancellationToken cancellationToken = default) =>
        db.Agreements.AnyAsync(agreement =>
            agreement.Id == agreementId &&
            (agreement.OwnerId == userId || db.Signatories.Any(signatory =>
                signatory.AgreementId == agreementId &&
                signatory.UserId == userId &&
                (signatory.Role == SignatoryRole.Collaborator ||
                 signatory.Role == SignatoryRole.Signer ||
                 signatory.Role == SignatoryRole.Viewer))),
            cancellationToken);

    /// <inheritdoc />
    public Task<bool> CanMutateAsync(Guid agreementId, Guid userId, CancellationToken cancellationToken = default) =>
        db.Agreements.AnyAsync(agreement =>
            agreement.Id == agreementId &&
            (agreement.OwnerId == userId || db.Signatories.Any(signatory =>
                signatory.AgreementId == agreementId &&
                signatory.UserId == userId &&
                signatory.Role == SignatoryRole.Collaborator)),
            cancellationToken);
}