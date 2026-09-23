using System.Security.Claims;

namespace Accordly.Api.Services;

/// <summary>Resolves the authenticated API user from the canonical JWT claim.</summary>
public interface ICurrentUserService
{
    Guid GetRequiredUserId(ClaimsPrincipal principal);
}

/// <inheritdoc />
public sealed class CurrentUserService : ICurrentUserService
{
    /// <inheritdoc />
    public Guid GetRequiredUserId(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("An authenticated user is required.");
        }

        var claimValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claimValue, out var userId) && userId != Guid.Empty
            ? userId
            : throw new UnauthorizedAccessException("The authenticated user claim is missing or malformed.");
    }
}