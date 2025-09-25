using System.Text.Json;

namespace CodingJournal.API.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";
            
            var response = new { errors = new[] { "An internal server error occurred." } };
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}