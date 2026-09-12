using Carter;

namespace Accordly.Api.Modules;

/// <summary>Version endpoints.</summary>
public sealed class VersionsModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/agreements/{id:guid}/versions").RequireAuthorization();

        group.MapGet("/", () => Results.Ok("stub"));
        group.MapPost("/", () => Results.Ok("stub"));
        group.MapGet("/{versionId:guid}", () => Results.Ok("stub"));
        group.MapGet("/diff", () => Results.Ok("stub"));
    }
}