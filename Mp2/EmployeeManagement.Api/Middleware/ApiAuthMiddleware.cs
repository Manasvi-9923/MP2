using EmployeeManagement.Api.Security;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace EmployeeManagement.Api.Middleware;

public class ApiAuthMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null;

        if (allowAnonymous || IsSwaggerRequest(context.Request.Path))
        {
            await next(context);
            return;
        }

        var header = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(header) || !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await WriteUnauthorized(context, "Missing bearer token.");
            return;
        }

        var token = header["Bearer ".Length..].Trim();
        if (!tokenService.TryValidateToken(token, out var username, out var role))
        {
            await WriteUnauthorized(context, "Invalid or expired token.");
            return;
        }

        context.SetCurrentUser(new CurrentUserContext(username, role));

        var roleRequirement = endpoint?.Metadata.GetMetadata<RequireRoleAttribute>();
        if (roleRequirement is not null &&
            !string.Equals(roleRequirement.Role, role, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "You do not have permission to perform this action." });
            return;
        }

        await next(context);
    }

    private static bool IsSwaggerRequest(PathString path)
    {
        return path.StartsWithSegments("/swagger");
    }

    private static async Task WriteUnauthorized(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { message });
    }
}
