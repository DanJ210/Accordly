using Carter;

namespace Accordly.Api.Modules;

/// <summary>Signatory endpoints.</summary>
public sealed class SignatoriesModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/agreements/{id:guid}/signatories").RequireAuthorization();

        group.MapGet("/", () => Results.Ok("stub"));
        group.MapPost("/", () => Results.Ok("stub"));
        group.MapDelete("/{sigId:guid}", () => Results.Ok("stub"));
        group.MapPost("/{sigId:guid}/sign", () => Results.Ok("stub"));

        app.MapGet("/api/v1/sign/{token}", () => Results.Ok("stub"));
        app.MapPost("/api/v1/sign/{token}", () => Results.Ok("stub"));
    }
}