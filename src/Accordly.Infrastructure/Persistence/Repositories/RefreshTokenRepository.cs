using Accordly.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Accordly.Infrastructure.Persistence;

/// <summary>EF Core refresh-token repository.</summary>
public sealed class RefreshTokenRepository(AccordlyDbContext db) : IRefreshTokenRepository
{
    /// <inheritdoc />
    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        db.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default) =>
        await db.RefreshTokens.AddAsync(token, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        db.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }
}
