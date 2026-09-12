using Accordly.Domain.Common;
using Accordly.Domain.Enums;

namespace Accordly.Domain.Entities;

/// <summary>A versioned agreement between parties.</summary>
public sealed class Agreement : Entity
{
    /// <summary>Gets or sets the title.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Gets the lifecycle status.</summary>
    public AgreementStatus Status { get; private set; } = AgreementStatus.Draft;
    /// <summary>Gets or sets the owner identifier.</summary>
    public Guid OwnerId { get; set; }
    /// <summary>Gets or sets the organization identifier.</summary>
    public Guid? OrganizationId { get; set; }
    /// <summary>Gets or sets the current version identifier.</summary>
    public Guid CurrentVersionId { get; set; }
    /// <summary>Gets or sets the last update timestamp.</summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    /// <summary>Gets or sets the optional expiry timestamp.</summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>Moves the agreement to the next valid lifecycle status.</summary>
    public void TransitionTo(AgreementStatus newStatus)
    {
        var isValidTransition = (Status, newStatus) switch
        {
            (AgreementStatus.Draft, AgreementStatus.PendingSignatures) => true,
            (AgreementStatus.PendingSignatures, AgreementStatus.Active) => true,
            (AgreementStatus.Active, AgreementStatus.Expired) => true,
            (AgreementStatus.Active, AgreementStatus.Terminated) => true,
            _ => false
        };

        if (!isValidTransition)
        {
            throw new InvalidOperationException($"Cannot transition agreement from {Status} to {newStatus}.");
        }

        Status = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
