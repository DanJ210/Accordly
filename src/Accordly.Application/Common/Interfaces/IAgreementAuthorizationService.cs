namespace Accordly.Application.Common.Interfaces;

/// <summary>Determines whether an authenticated user may access an agreement.</summary>
public interface IAgreementAuthorizationService
{
    /// <summary>Returns whether the user may read the agreement.</summary>
    Task<bool> CanReadAsync(Guid agreementId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Returns whether the user may mutate the agreement.</summary>
    Task<bool> CanMutateAsync(Guid agreementId, Guid userId, CancellationToken cancellationToken = default);
}