namespace Accordly.Domain.Events;

public sealed record SignatorySignedEvent(
    Guid AgreementId,
    Guid SignatoryId,
    Guid? VersionSignedId,
    DateTimeOffset SignedAt);