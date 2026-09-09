using Accordly.Domain.Entities;

namespace Accordly.Application.Common.Interfaces;

/// <summary>Persistence operations for agreements.</summary>
public interface IAgreementRepository
{
    /// <summary>Gets an agreement by identifier.</summary>
    Task<Agreement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>Gets agreements owned by a user.</summary>
    Task<IReadOnlyList<Agreement>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <summary>Adds an agreement.</summary>
    Task AddAsync(Agreement agreement, CancellationToken cancellationToken = default);
    /// <summary>Updates an agreement.</summary>
    Task UpdateAsync(Agreement agreement, CancellationToken cancellationToken = default);
    /// <summary>Deletes an agreement.</summary>
    Task DeleteAsync(Agreement agreement, CancellationToken cancellationToken = default);
}
