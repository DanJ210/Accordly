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

            var domainUser = new User
            {
                Id = applicationUser.Id,
                Email = email,
                DisplayName = displayName,
                PublicKey = string.Empty
            };

            try
            {
                await userRepository.AddAsync(domainUser, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await userManager.DeleteAsync(applicationUser);
                throw;
            }

            var response = CreateAuthResponse(tokenService, configuration, applicationUser);
            return Results.Created("/api/v1/auth/register", response);
        });

        group.MapPost("/login", async (LoginRequest request,
            UserManager<ApplicationUser> userManager,
            TokenService tokenService,
            IConfiguration configuration) =>
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
                return Results.Unauthenticated();
            }

            var isValid = await userManager.CheckPasswordAsync(user, password);
            if (!isValid)
            {
                return Results.Unauthenticated();
            }

            var response = CreateAuthResponse(tokenService, configuration, user);
            return Results.Ok(response);
        });

        group.MapPost("/refresh", (RefreshTokenRequest request) => Results.Ok("stub"));
        group.MapPost("/logout", (RefreshTokenRequest request) => Results.NoContent());
    }

    private static AuthResponse CreateAuthResponse(TokenService tokenService, IConfiguration configuration, ApplicationUser user)
    {
        var expiryMinutes = int.TryParse(configuration["Jwt:ExpiryMinutes"], out var minutes) ? minutes : 60;
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes);
        var accessToken = tokenService.CreateAccessToken(user.Id, user.Email ?? string.Empty, user.DisplayName);
        var refreshToken = tokenService.CreateRefreshToken();
        return new AuthResponse(accessToken, refreshToken, expiresAt);
    }
}