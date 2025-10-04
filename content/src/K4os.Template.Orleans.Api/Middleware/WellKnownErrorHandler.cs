using K4os.Template.Orleans.Core;
using K4os.Template.Orleans.Interfaces.Messages;
using Microsoft.AspNetCore.Http;

namespace K4os.Template.Orleans.Api.Middleware;

public class WellKnownErrorHandler
{
    private readonly RequestDelegate _next;

    public WellKnownErrorHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (WellKnownError e)
        {
            WriteError(httpContext, e);
        }
        catch (Exception e)
        {
            WriteError(httpContext, e);
        }
    }

    private static void WriteError(HttpContext httpContext, Exception exception)
    {
        var response = httpContext.Response;
        response.StatusCode = exception switch {
            UnauthorisedError => StatusCodes.Status401Unauthorized,
            ForbiddenError => StatusCodes.Status403Forbidden,
            NotFoundError => StatusCodes.Status404NotFound,
            BadRequestError => StatusCodes.Status400BadRequest,
            TimeoutError => StatusCodes.Status408RequestTimeout,
            _ => StatusCodes.Status500InternalServerError,
        };
        response.ContentType = "text/plain";

#if DEBUG
        response.WriteAsync(exception.Explain());
#else
		response.WriteAsync(exception.Message);
#endif
    }
}
