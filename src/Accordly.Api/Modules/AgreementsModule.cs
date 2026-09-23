using Accordly.Application.Agreements.Commands.CreateAgreement;
using Accordly.Application.Agreements.Queries.GetAgreement;
using Accordly.Application.Agreements.Queries.ListAgreements;
using Accordly.Api.Services;
using Accordly.Contracts.Agreements;
using Carter;
using MediatR;

namespace Accordly.Api.Modules;

/// <summary>Agreement endpoints.</summary>
public sealed class AgreementsModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/agreements").RequireAuthorization();
        group.MapGet("/", async (HttpContext context, ICurrentUserService currentUser, ISender sender, CancellationToken cancellationToken) => Results.Ok(await sender.Send(new ListAgreementsQuery(currentUser.GetRequiredUserId(context.User)), cancellationToken)));
        group.MapPost("/", async (HttpContext context, ICurrentUserService currentUser, CreateAgreementRequest request, ISender sender, CancellationToken cancellationToken) => Results.Created("/api/v1/agreements", await sender.Send(new CreateAgreementCommand(request.Title, currentUser.GetRequiredUserId(context.User), request.ExpiresAt), cancellationToken)));
        group.MapGet("/{id:guid}", async (Guid id, HttpContext context, ICurrentUserService currentUser, ISender sender, CancellationToken cancellationToken) => { var result = await sender.Send(new GetAgreementQuery(id, currentUser.GetRequiredUserId(context.User)), cancellationToken); return result is null ? Results.NotFound() : Results.Ok(result); });
        group.MapPatch("/{id:guid}", (Guid id) => Results.Ok(new { id, status = "stub" }));
        group.MapDelete("/{id:guid}", (Guid id) => Results.NoContent());
    }
}
