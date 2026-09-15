using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Accordly.Application.Common.Services;

/// <summary>Creates signed access tokens and refresh tokens.</summary>
public sealed class TokenService(IConfiguration configuration)
{
    private const int RefreshTokenBytes = 32;

    /// <summary>Creates a signed JWT access token for the supplied user.</summary>
    public string CreateAccessToken(Guid userId, string email, string displayName)
    {
        var secret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret is required.");
        if (secret.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Secret must be at least 32 characters long.");
        }

        var issuer = configuration["Jwt:Issuer"] ?? "accordly";
        var audience = configuration["Jwt:Audience"] ?? "accordly-client";
        var expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var minutes) ? minutes : 60;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("displayName", displayName),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>Creates a cryptographically random refresh token value.</summary>
    public string CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(RefreshTokenBytes);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>Hashes the refresh-token value for persistence.</summary>
    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    /// <summary>Returns true when the supplied raw token matches the stored hash.</summary>
    public bool IsRefreshTokenValid(string rawToken, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(rawToken) || string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        return string.Equals(HashToken(rawToken), storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
