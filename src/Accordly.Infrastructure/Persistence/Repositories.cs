using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accordly.Infrastructure.Persistence;

/// <summary>EF Core agreement repository.</summary>
public sealed class AgreementRepository(AccordlyDbContext db) : IAgreementRepository
{
    /// <inheritdoc />
    public Task<Agreement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => db.Agreements.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    /// <inheritdoc />
    public async Task<IReadOnlyList<Agreement>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken = default) => await db.Agreements.Where(item => item.OwnerId == userId).ToListAsync(cancellationToken);
    /// <inheritdoc />
    public async Task AddAsync(Agreement agreement, CancellationToken cancellationToken = default) => await db.Agreements.AddAsync(agreement, cancellationToken);
    /// <inheritdoc />
    public Task UpdateAsync(Agreement agreement, CancellationToken cancellationToken = default) { db.Agreements.Update(agreement); return Task.CompletedTask; }
    /// <inheritdoc />
    public Task DeleteAsync(Agreement agreement, CancellationToken cancellationToken = default) { db.Agreements.Remove(agreement); return Task.CompletedTask; }
}

/// <summary>EF Core domain user repository.</summary>
public sealed class UserRepository(AccordlyDbContext db) : IUserRepository
{
    /// <inheritdoc />
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => db.Users.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) => db.Users.FirstOrDefaultAsync(item => item.Email == email, cancellationToken);
    /// <inheritdoc />
    public async Task AddAsync(User user, CancellationToken cancellationToken = default) => await db.Users.AddAsync(user, cancellationToken);
}

/// <summary>EF Core unit of work.</summary>
public sealed class UnitOfWork(AccordlyDbContext db) : IUnitOfWork
{
    /// <inheritdoc />
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
