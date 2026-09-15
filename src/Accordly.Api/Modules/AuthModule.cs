using Accordly.Application.Common.Interfaces;
using Accordly.Application.Common.Services;
using Accordly.Contracts.Auth;
using Accordly.Domain.Entities;
using Accordly.Infrastructure.Persistence;
using Carter;
using Microsoft.AspNetCore.Identity;

namespace Accordly.Api.Modules;

/// <summary>Authentication endpoints.</summary>
public sealed class AuthModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth");

        group.MapPost("/register", async (RegisterRequest request,
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            TokenService tokenService,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository,
            CancellationToken cancellationToken) =>
        {
            var email = request.Email?.Trim();
            var displayName = request.DisplayName?.Trim();
            var password = request.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(password))
            {
                return Results.BadRequest(new { errors = new[] { "Email, display name, and password are required." } });
            }

            if (await userManager.FindByEmailAsync(email) is not null)
            {
                return Results.BadRequest(new { errors = new[] { "An account with this email already exists." } });
            }

            var applicationUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                DisplayName = displayName
            };

            var createResult = await userManager.CreateAsync(applicationUser, password);
            if (!createResult.Succeeded)
            {
                return Results.BadRequest(new { errors = createResult.Errors.Select(error => error.Description) });
            }

            var domainUser = new User(applicationUser.Id)
            {
                Email = email,
                DisplayName = displayName,
                PublicKey = string.Empty
            };

            var response = CreateAuthResponse(tokenService, configuration, applicationUser);

            try
            {
                await userRepository.AddAsync(domainUser, cancellationToken);
                await refreshTokenRepository.AddAsync(CreateRefreshTokenRecord(tokenService, configuration, applicationUser.Id, response.RefreshToken), cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await userManager.DeleteAsync(applicationUser);
                throw;
            }

            return Results.Created("/api/v1/auth/register", response);
        });

        group.MapPost("/login", async (LoginRequest request,
            UserManager<ApplicationUser> userManager,
            TokenService tokenService,
            IConfiguration configuration,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            CancellationToken cancellationToken) =>
        {
            var email = request.Email?.Trim();
            var password = request.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return Results.BadRequest(new { errors = new[] { "Email and password are required." } });
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var isValid = await userManager.CheckPasswordAsync(user, password);
            if (!isValid)
            {
                return Results.Unauthorized();
            }

            var response = CreateAuthResponse(tokenService, configuration, user);
            await refreshTokenRepository.AddAsync(CreateRefreshTokenRecord(tokenService, configuration, user.Id, response.RefreshToken), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Results.Ok(response);
        });

        group.MapPost("/refresh", async (RefreshTokenRequest request,
            UserManager<ApplicationUser> userManager,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            TokenService tokenService,
            IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Results.BadRequest(new { errors = new[] { "Refresh token is required." } });
            }

            var tokenHash = tokenService.HashToken(request.RefreshToken);
            var currentToken = await refreshTokenRepository.GetByHashAsync(tokenHash, cancellationToken);

            if (currentToken is null || currentToken.RevokedAt is not null || currentToken.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                return Results.Unauthorized();
            }

            var user = await userManager.FindByIdAsync(currentToken.UserId.ToString());
            if (user is null)
            {
                return Results.Unauthorized();
            }

            var replacementTokenValue = tokenService.CreateRefreshToken();
            var replacementToken = new RefreshToken
            {
                UserId = currentToken.UserId,
                TokenHash = tokenService.HashToken(replacementTokenValue),
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(GetRefreshExpiryHours(configuration))
            };

            currentToken.RevokedAt = DateTimeOffset.UtcNow;
            currentToken.ReplacedByTokenId = replacementToken.Id;

            await refreshTokenRepository.AddAsync(replacementToken, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var response = CreateAuthResponse(tokenService, configuration, user, replacementTokenValue);
            return Results.Ok(response);
        });

        group.MapPost("/logout", async (RefreshTokenRequest request,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            TokenService tokenService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Results.BadRequest(new { errors = new[] { "Refresh token is required." } });
            }

            var tokenHash = tokenService.HashToken(request.RefreshToken);
            var token = await refreshTokenRepository.GetByHashAsync(tokenHash, cancellationToken);

            if (token is null || token.RevokedAt is not null)
            {
                return Results.NoContent();
            }

            token.RevokedAt = DateTimeOffset.UtcNow;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Results.NoContent();
        });
    }

    private static AuthResponse CreateAuthResponse(TokenService tokenService, IConfiguration configuration, ApplicationUser user, string? refreshToken = null)
    {
        var expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var minutes) ? minutes : 60;
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);
        var accessToken = tokenService.CreateAccessToken(user.Id, user.Email ?? string.Empty, user.DisplayName);
        return new AuthResponse(accessToken, refreshToken ?? tokenService.CreateRefreshToken(), expiresAt);
    }

    private static int GetRefreshExpiryHours(IConfiguration configuration)
    {
        return int.TryParse(configuration["RefreshToken:ExpiryHours"], out var hours) && hours > 0
            ? hours
            : 168;
    }

    private static RefreshToken CreateRefreshTokenRecord(TokenService tokenService, IConfiguration configuration, Guid userId, string rawToken)
    {
        return new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenService.HashToken(rawToken),
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(GetRefreshExpiryHours(configuration))
        };
    }
}