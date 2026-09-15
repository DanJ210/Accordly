using Microsoft.AspNetCore.Identity;

namespace Accordly.Infrastructure.Persistence;

/// <summary>Persisted refresh token bound to an identity user.</summary>
public sealed class RefreshToken
{
    /// <summary>Gets the token identifier.</summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>Gets or sets the owning identity user id.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the SHA-256 hash of the refresh token.</summary>
    public string TokenHash { get; set; } = string.Empty;

    /// <summary>Gets the timestamp when the token was created.</summary>
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    /// <summary>Gets or sets the token expiry timestamp.</summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>Gets or sets the timestamp when the token was revoked.</summary>
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>Gets or sets the replacement token id when rotated.</summary>
    public Guid? ReplacedByTokenId { get; set; }

    /// <summary>Gets or sets the owning identity user.</summary>
    public ApplicationUser? User { get; set; }
}
