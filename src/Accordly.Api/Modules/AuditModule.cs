using Carter;

namespace Accordly.Api.Modules;

/// <summary>Audit endpoints.</summary>
public sealed class AuditModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/agreements/{id:guid}/audit", () => Results.Ok("stub"))
            .RequireAuthorization();
    }
}