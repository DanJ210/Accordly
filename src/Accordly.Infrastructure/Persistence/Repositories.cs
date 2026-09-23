using Accordly.Application.Common.Interfaces;
using Accordly.Domain.Entities;
using Accordly.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Accordly.Infrastructure.Persistence;

/// <summary>EF Core agreement repository.</summary>
public sealed class AgreementRepository(AccordlyDbContext db) : IAgreementRepository
{
    /// <inheritdoc />
    public Task<Agreement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => db.Agreements.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    /// <inheritdoc />
    public async Task<IReadOnlyList<Agreement>> GetAllForUserAsync(Guid userId, CancellationToken cancellationToken = default) => await db.Agreements
        .Where(agreement => agreement.OwnerId == userId || db.Signatories.Any(signatory =>
            signatory.AgreementId == agreement.Id && signatory.UserId == userId &&
            (signatory.Role == SignatoryRole.Collaborator || signatory.Role == SignatoryRole.Signer || signatory.Role == SignatoryRole.Viewer)))
        .OrderByDescending(agreement => agreement.UpdatedAt)
        .ToListAsync(cancellationToken);
    /// <inheritdoc />
    public Task<AgreementVersion?> GetVersionAsync(Guid agreementId, Guid versionId, CancellationToken cancellationToken = default) => db.AgreementVersions
        .FirstOrDefaultAsync(version => version.AgreementId == agreementId && version.Id == versionId, cancellationToken);
    /// <inheritdoc />
    public Task<IReadOnlyList<AgreementVersion>> GetVersionsForAgreementAsync(Guid agreementId, CancellationToken cancellationToken = default) => db.AgreementVersions
        .Where(version => version.AgreementId == agreementId)
        .OrderBy(version => version.VersionNumber)
        .ToListAsync(cancellationToken)
        .ContinueWith(task => (IReadOnlyList<AgreementVersion>)task.Result, cancellationToken);
    /// <inheritdoc />
    public async Task<int> GetNextVersionNumberAsync(Guid agreementId, CancellationToken cancellationToken = default)
    {
        var latestVersionNumber = await db.AgreementVersions
            .Where(version => version.AgreementId == agreementId)
            .MaxAsync(version => (int?)version.VersionNumber, cancellationToken);

        return (latestVersionNumber ?? 0) + 1;
    }
    /// <inheritdoc />
    public async Task AddAsync(Agreement agreement, CancellationToken cancellationToken = default) => await db.Agreements.AddAsync(agreement, cancellationToken);
    /// <inheritdoc />
    public async Task AddVersionAsync(AgreementVersion version, CancellationToken cancellationToken = default) => await db.AgreementVersions.AddAsync(version, cancellationToken);
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
