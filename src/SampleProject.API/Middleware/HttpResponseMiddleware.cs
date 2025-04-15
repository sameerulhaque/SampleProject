using Microsoft.AspNetCore.Http;
using SampleProject.Core.Exceptions;
using SampleProject.Infrastructure.Exceptions;

namespace SampleProject.API.Middleware;

public class HttpResponseMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        await next(context);

        switch (context.Response.StatusCode)
        {
            case StatusCodes.Status401Unauthorized: throw new UnauthorizedException("Unauthorized");
            case StatusCodes.Status403Forbidden: throw new ForbiddenException("Forbidden");
            case StatusCodes.Status405MethodNotAllowed: throw new MethodNotAllowedException("MethodNotAllowed");
        }
    }
}
