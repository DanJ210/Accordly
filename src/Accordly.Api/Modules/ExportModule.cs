using Carter;

namespace Accordly.Api.Modules;

/// <summary>Export endpoints.</summary>
public sealed class ExportModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/agreements/{id:guid}/export").RequireAuthorization();

        group.MapGet("/pdf", () => Results.Ok("stub"));
        group.MapGet("/json", () => Results.Ok("stub"));
    }
}