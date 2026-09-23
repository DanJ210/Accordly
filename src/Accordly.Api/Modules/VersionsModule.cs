using Accordly.Api.Services;
using Accordly.Application.Agreements.Queries.GetVersion;
using Accordly.Application.Agreements.Queries.ListVersions;
using Carter;
using MediatR;

namespace Accordly.Api.Modules;

/// <summary>Version endpoints.</summary>
public sealed class VersionsModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/agreements/{id:guid}/versions").RequireAuthorization();

        group.MapGet("/", async (Guid id, HttpContext context, ICurrentUserService currentUser, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ListVersionsQuery(id, currentUser.GetRequiredUserId(context.User)), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
        group.MapGet("/{versionId:guid}", async (Guid id, Guid versionId, HttpContext context, ICurrentUserService currentUser, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetVersionQuery(id, currentUser.GetRequiredUserId(context.User), versionId), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
        group.MapPost("/", () => Results.Ok("stub"));
        group.MapGet("/diff", () => Results.Ok("stub"));
    }
}