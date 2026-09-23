using Accordly.Domain.Entities;

namespace Accordly.Application.Common.Interfaces;

/// <summary>Records immutable agreement audit events.</summary>
public interface IAuditEventRecorder
{
    /// <summary>Adds an audit event to the current unit of work.</summary>
    Task RecordAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default);
}