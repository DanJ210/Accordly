namespace Accordly.Domain.Events;

public sealed record AgreementVersionCreatedEvent(
    Guid AgreementId,
    Guid VersionId,
    int VersionNumber,
    Guid AuthorId,
    DateTimeOffset CreatedAt);