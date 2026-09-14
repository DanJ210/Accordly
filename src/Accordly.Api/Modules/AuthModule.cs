using Accordly.Contracts.Auth;
using Carter;

namespace Accordly.Api.Modules;

/// <summary>Authentication endpoints.</summary>
public sealed class AuthModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth");

        group.MapPost("/register", (RegisterRequest request) => Results.Ok("stub"));
        group.MapPost("/login", (LoginRequest request) => Results.Ok("stub"));
        group.MapPost("/refresh", (RefreshTokenRequest request) => Results.Ok("stub"));
        group.MapPost("/logout", (RefreshTokenRequest request) => Results.NoContent());
    }
}