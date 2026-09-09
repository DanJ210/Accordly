using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Accordly.Api.Middleware;

/// <summary>Converts application exceptions into consistent HTTP responses.</summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment environment)
{
    /// <summary>Processes one request.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (ValidationException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new { errors = exception.Errors.GroupBy(error => error.PropertyName).ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage)) });
        }
        catch (KeyNotFoundException) { context.Response.StatusCode = (int)HttpStatusCode.NotFound; }
        catch (Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = environment.IsDevelopment() ? exception.Message : "An unexpected error occurred." });
        }
    }
}
