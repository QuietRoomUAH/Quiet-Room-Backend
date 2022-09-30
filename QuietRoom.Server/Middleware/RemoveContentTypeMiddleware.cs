namespace QuietRoom.Server.Middleware;

public class RemoveContentTypeMiddleware
{
    private readonly RequestDelegate _next;

    public RemoveContentTypeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = context.Request;
        if (request.Method == "GET" && request.Headers.ContainsKey("Content-Type"))
        {
            request.Headers.Remove("Content-Type");
        }

        await _next(context);
    }
}

public static class RemoveContentTypeMiddlewareExtensions
{
    public static IApplicationBuilder UseRemoveContentTypeMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RemoveContentTypeMiddleware>();
    }
}
