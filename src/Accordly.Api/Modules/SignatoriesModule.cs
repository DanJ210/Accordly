using Accordly.Api.Services;
using Accordly.Application.Agreements.Commands.DeleteSignatory;
using Accordly.Application.Agreements.Commands.InviteSignatory;
using Accordly.Application.Agreements.Commands.SubmitGuestSignature;
using Accordly.Application.Agreements.Commands.SubmitRegisteredSignature;
using Accordly.Application.Agreements.Queries.ListSignatories;
using Accordly.Application.Agreements.Queries.ResolveGuestSignatory;
using Accordly.Contracts.Signatories;
using Carter;
using MediatR;

namespace Accordly.Api.Modules;

/// <summary>Signatory endpoints.</summary>
public sealed class SignatoriesModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/agreements/{id:guid}/signatories").RequireAuthorization();

        group.MapGet("/", async (Guid id, HttpContext context, ICurrentUserService currentUser, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ListSignatoriesQuery(id, currentUser.GetRequiredUserId(context.User)), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
        group.MapPost("/", async (Guid id, HttpContext context, InviteSignatoryRequest request, ICurrentUserService currentUser, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new InviteSignatoryCommand(id, currentUser.GetRequiredUserId(context.User), request.Email, request.Role), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
        group.MapDelete("/{sigId:guid}", async (Guid id, Guid sigId, HttpContext context, ICurrentUserService currentUser, ISender sender, CancellationToken cancellationToken) =>
        {
            var removed = await sender.Send(new DeleteSignatoryCommand(id, sigId, currentUser.GetRequiredUserId(context.User)), cancellationToken);
            return removed ? Results.NoContent() : Results.NotFound();
        });
        group.MapPost("/{sigId:guid}/sign", async (Guid id, Guid sigId, HttpContext context, SubmitSignatureRequest request, ICurrentUserService currentUser, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new SubmitRegisteredSignatureCommand(id, sigId, currentUser.GetRequiredUserId(context.User), request.SignatureValue, context.Connection.RemoteIpAddress?.ToString()), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        app.MapGet("/api/v1/sign/{token}", async (string token, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ResolveGuestSignatoryQuery(token), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
        app.MapPost("/api/v1/sign/{token}", async (string token, SubmitSignatureRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new SubmitGuestSignatureCommand(token, request.SignatureValue, null), cancellationToken);
            return result is null ? Results.NotFound() : Results.Ok(result);
        });
    }
}