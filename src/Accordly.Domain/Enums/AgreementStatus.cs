namespace Accordly.Domain.Enums;

/// <summary>Lifecycle states for an agreement.</summary>
public enum AgreementStatus
{
    Draft,
    PendingSignatures,
    Active,
    Expired,
    Terminated
}
