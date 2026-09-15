namespace Accordly.Infrastructure.Persistence;

/// <summary>Persistence operations for refresh tokens.</summary>
public interface IRefreshTokenRepository
{
    /// <summary>Gets a refresh token by its persisted hash.</summary>
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    /// <summary>Adds a refresh token.</summary>
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
    /// <summary>Updates a refresh token.</summary>
    Task UpdateAsync(RefreshToken token, CancellationToken cancellationToken = default);
}
