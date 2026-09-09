using Accordly.Domain.Common;

namespace Accordly.Domain.Entities;

/// <summary>Immutable audit record for agreement activity.</summary>
public sealed class AuditEvent : Entity
{
    /// <summary>Gets or sets the agreement identifier.</summary>
    public Guid AgreementId { get; set; }
    /// <summary>Gets or sets the optional actor identifier.</summary>
    public Guid? ActorId { get; set; }
    /// <summary>Gets or sets the event type.</summary>
    public string EventType { get; set; } = string.Empty;
    /// <summary>Gets or sets the JSON event payload.</summary>
    public string? Payload { get; set; }
    /// <summary>Gets or sets the occurrence timestamp.</summary>
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;
    /// <summary>Gets or sets the source IP address.</summary>
    public string? IpAddress { get; set; }
}
