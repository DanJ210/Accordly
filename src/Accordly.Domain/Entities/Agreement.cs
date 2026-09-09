using Accordly.Domain.Common;
using Accordly.Domain.Enums;

namespace Accordly.Domain.Entities;

/// <summary>A versioned agreement between parties.</summary>
public sealed class Agreement : Entity
{
    /// <summary>Gets or sets the title.</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Gets or sets the lifecycle status.</summary>
    public AgreementStatus Status { get; set; } = AgreementStatus.Draft;
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
}
