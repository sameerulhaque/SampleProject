using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SampleProject.Core.Exceptions;
using SampleProject.Infrastructure.Authentication;
using SampleProject.Infrastructure.Exceptions;

namespace SampleProject.API.Middleware;

public class RateLimitMiddleware(RequestDelegate next, IMemoryCache memoryCache)
{
    private readonly TimeSpan timeLimit = TimeSpan.FromMinutes(1);
    private readonly int countLimit = 100;

    public async Task Invoke(HttpContext context)
    {
        var key = UserInfoHelper.IPAddress;

        memoryCache.TryGetValue(key, out int requestCount);

        if (requestCount > countLimit)
        {
            throw new TooManyRequestException("Too many requests");
        }
        else
        {
            await next(context);
        }

        requestCount++;
        memoryCache.Set(key, requestCount, timeLimit);
    }
}