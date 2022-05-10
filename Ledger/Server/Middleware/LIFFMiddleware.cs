using Ledger.Server.Service.LIFF;

namespace Ledger.Server.Middleware;

public class LIFFMiddleware
{
    private readonly RequestDelegate _next;
    public LIFFMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // IMessageWriter is injected into InvokeAsync
    public async Task InvokeAsync(HttpContext httpContext, UserProfileService userProfile)
    {
        await userProfile.SetUserProfile(httpContext);
        await _next(httpContext);
    }
}
public static class LIFFMiddlewareExtensions
{
    public static IApplicationBuilder UseLIFFMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LIFFMiddleware>();
    }
}
