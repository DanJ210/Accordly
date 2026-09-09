using Accordly.Domain.Enums;

namespace Accordly.Domain.Events;

public sealed record AgreementStatusChangedEvent(
    Guid AgreementId,
    AgreementStatus PreviousStatus,
    AgreementStatus NewStatus,
    DateTimeOffset OccurredAt);