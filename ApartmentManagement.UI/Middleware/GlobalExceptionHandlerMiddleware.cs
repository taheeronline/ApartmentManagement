using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ApartmentManagement.UI.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var errorId = Guid.NewGuid().ToString("N");

            // Map exception types to status codes and messages
            var (statusCode, message, logLevel) = ex switch
            {
                ArgumentException ae => ((int)HttpStatusCode.BadRequest, ae.Message, LogLevel.Warning),
                InvalidOperationException ioe => ((int)HttpStatusCode.Conflict, ioe.Message, LogLevel.Warning),
                _ => ((int)HttpStatusCode.InternalServerError, "An unexpected error occurred. Please contact support.", LogLevel.Error)
            };

            // Log with appropriate level and include ErrorId for correlation
            if (logLevel == LogLevel.Error)
                _logger.LogError(ex, "Unhandled exception occurred. ErrorId: {ErrorId}", errorId);
            else
                _logger.LogWarning(ex, "Handled exception occurred. ErrorId: {ErrorId}", errorId);

            if (IsApiOrAjaxRequest(context))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var payload = new
                {
                    Message = message,
                    ErrorId = errorId
                };

                var json = JsonSerializer.Serialize(payload);
                await context.Response.WriteAsync(json);
            }
            else
            {
                // For non-API requests redirect to the Error page and include ErrorId and message as query
                var redirectPath = $"/Error?errorId={WebUtility.UrlEncode(errorId)}&message={WebUtility.UrlEncode(message)}";
                if (!context.Response.HasStarted)
                {
                    context.Response.Redirect(redirectPath);
                }
            }
        }
    }

    private static bool IsApiOrAjaxRequest(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Requested-With", out var header) && header == "XMLHttpRequest")
            return true;

        if (context.Request.Headers.TryGetValue("Accept", out var accept) && accept.Any(a => a.Contains("application/json")))
            return true;

        if (context.Request.Path.HasValue && context.Request.Path.Value!.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}
