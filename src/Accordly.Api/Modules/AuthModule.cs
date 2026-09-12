using Carter;

namespace Accordly.Api.Modules;

/// <summary>Authentication endpoints.</summary>
public sealed class AuthModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth");

        group.MapPost("/register", () => Results.Ok("stub"));
        group.MapPost("/login", () => Results.Ok("stub"));
        group.MapPost("/refresh", () => Results.Ok("stub"));
        group.MapPost("/logout", () => Results.Ok("stub"));
    }
}