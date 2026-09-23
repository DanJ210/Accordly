using Accordly.Domain.Entities;

namespace Accordly.Application.Common.Interfaces;

/// <summary>Persistence operations for agreements.</summary>
public interface IAgreementRepository
{
    /// <summary>Gets an agreement by identifier.</summary>
    Task<Agreement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>Gets agreements visible to a user.</summary>
    Task<IReadOnlyList<Agreement>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <summary>Gets the current version for an agreement.</summary>
    Task<AgreementVersion?> GetVersionAsync(Guid agreementId, Guid versionId, CancellationToken cancellationToken = default);
    /// <summary>Gets the next immutable version number for an agreement.</summary>
    Task<int> GetNextVersionNumberAsync(Guid agreementId, CancellationToken cancellationToken = default);
    /// <summary>Adds an agreement.</summary>
    Task AddAsync(Agreement agreement, CancellationToken cancellationToken = default);
    /// <summary>Adds a new agreement version.</summary>
    Task AddVersionAsync(AgreementVersion version, CancellationToken cancellationToken = default);
    /// <summary>Updates an agreement.</summary>
    Task UpdateAsync(Agreement agreement, CancellationToken cancellationToken = default);
    /// <summary>Deletes an agreement.</summary>
    Task DeleteAsync(Agreement agreement, CancellationToken cancellationToken = default);
}
