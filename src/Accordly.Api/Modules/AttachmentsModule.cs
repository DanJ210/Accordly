using Carter;

namespace Accordly.Api.Modules;

/// <summary>Attachment endpoints.</summary>
public sealed class AttachmentsModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/agreements/{id:guid}/attachments").RequireAuthorization();

        group.MapGet("/", () => Results.Ok("stub"));
        group.MapPost("/", () => Results.Ok("stub"));
        group.MapGet("/{attachId:guid}", () => Results.Ok("stub"));
        group.MapDelete("/{attachId:guid}", () => Results.Ok("stub"));
    }
}