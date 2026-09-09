using Accordly.Domain.Entities;

namespace Accordly.Application.Common.Interfaces;

/// <summary>Persistence operations for domain users.</summary>
public interface IUserRepository
{
    /// <summary>Gets a user by identifier.</summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>Gets a user by email.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    /// <summary>Adds a user.</summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}
