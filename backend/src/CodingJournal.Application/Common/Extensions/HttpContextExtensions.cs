using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CodingJournal.Application.Common.Extensions;

public static class HttpContextExtensions
{
    public static Result<string> GetCurrentUserId(this HttpContext? httpContext)
    {
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
        {
            return Result<string>.Failure("User is not authenticated.");
        }

        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Result<string>.Failure("User not found in token.");
        }
        
        return Result<string>.Success(userId);
    }
}