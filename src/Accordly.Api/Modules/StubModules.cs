using Carter;

namespace Accordly.Api.Modules;

internal static class StubRoutes
{
    public static void Add(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/agreements/{id:guid}/versions", () => Results.Ok(Array.Empty<object>())).RequireAuthorization();
        app.MapPost("/api/v1/agreements/{id:guid}/versions", () => Results.Ok("stub")).RequireAuthorization();
        app.MapGet("/api/v1/agreements/{id:guid}/versions/{versionId:guid}", () => Results.Ok("stub")).RequireAuthorization();
        app.MapGet("/api/v1/agreements/{id:guid}/versions/diff", () => Results.Ok("stub")).RequireAuthorization();
        app.MapGet("/api/v1/agreements/{id:guid}/signatories", () => Results.Ok(Array.Empty<object>())).RequireAuthorization();
        app.MapPost("/api/v1/agreements/{id:guid}/signatories", () => Results.Ok("stub")).RequireAuthorization();
        app.MapDelete("/api/v1/agreements/{id:guid}/signatories/{sigId:guid}", () => Results.NoContent()).RequireAuthorization();
        app.MapPost("/api/v1/agreements/{id:guid}/signatories/{sigId:guid}/sign", () => Results.Ok("stub")).RequireAuthorization();
        app.MapGet("/api/v1/sign/{token}", (string token) => Results.Ok(new { token }));
        app.MapPost("/api/v1/sign/{token}", (string token) => Results.Ok(new { token }));
        app.MapGet("/api/v1/agreements/{id:guid}/attachments", () => Results.Ok(Array.Empty<object>())).RequireAuthorization();
        app.MapPost("/api/v1/agreements/{id:guid}/attachments", () => Results.Ok("stub")).RequireAuthorization();
        app.MapGet("/api/v1/agreements/{id:guid}/attachments/{attachId:guid}", () => Results.Ok("stub")).RequireAuthorization();
        app.MapDelete("/api/v1/agreements/{id:guid}/attachments/{attachId:guid}", () => Results.NoContent()).RequireAuthorization();
        app.MapGet("/api/v1/agreements/{id:guid}/export/pdf", () => Results.File(Array.Empty<byte>(), "application/pdf", "agreement.pdf")).RequireAuthorization();
        app.MapGet("/api/v1/agreements/{id:guid}/export/json", () => Results.Ok(new { })).RequireAuthorization();
        app.MapGet("/api/v1/agreements/{id:guid}/audit", () => Results.Ok(Array.Empty<object>())).RequireAuthorization();
        app.MapPost("/api/v1/auth/register", () => Results.Ok("stub"));
        app.MapPost("/api/v1/auth/login", () => Results.Ok("stub"));
        app.MapPost("/api/v1/auth/refresh", () => Results.Ok("stub"));
        app.MapPost("/api/v1/auth/logout", () => Results.NoContent());
    }
}

/// <summary>Version endpoints.</summary>
public sealed class VersionsModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app) => StubRoutes.Add(app);
}
/// <summary>Signatory endpoints.</summary>
public sealed class SignatoriesModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app) { }
}
/// <summary>Attachment endpoints.</summary>
public sealed class AttachmentsModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app) { }
}
/// <summary>Export endpoints.</summary>
public sealed class ExportModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app) { }
}
/// <summary>Authentication endpoints.</summary>
public sealed class AuthModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app) { }
}
/// <summary>Audit endpoints.</summary>
public sealed class AuditModule : ICarterModule
{
    /// <inheritdoc />
    public void AddRoutes(IEndpointRouteBuilder app) { }
}
