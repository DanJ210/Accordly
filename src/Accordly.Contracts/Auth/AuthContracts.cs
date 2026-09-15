namespace Accordly.Contracts.Auth;

/// <summary>Registration request.</summary>
public sealed record RegisterRequest(string Email, string DisplayName, string Password);
/// <summary>Login request.</summary>
public sealed record LoginRequest(string Email, string Password);
/// <summary>Refresh-token request used by refresh and logout flows.</summary>
public sealed record RefreshTokenRequest(string RefreshToken);
/// <summary>Authentication response.</summary>
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
