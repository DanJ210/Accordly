using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;

namespace Accordly.Infrastructure.Persistence;

/// <summary>Persists append-only agreement audit events.</summary>
public sealed class AuditEventRecorder(AccordlyDbContext db) : IAuditEventRecorder
{
    /// <inheritdoc />
    public Task RecordAsync(AuditEvent auditEvent, CancellationToken cancellationToken = default) =>
        db.AuditEvents.AddAsync(auditEvent, cancellationToken).AsTask();
}