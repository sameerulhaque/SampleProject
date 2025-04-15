using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SampleProject.API.Attributes;
using SampleProject.Infrastructure.Authentication;
using SampleProject.Infrastructure.Caching;
using SampleProject.Infrastructure.Services;
using SampleProject.Shared.Models.Global;
using System.Security.Cryptography;
using System.Text;

namespace BuildingBlocks.Application.Middlewares;

public class IdempotentMiddleware
{
    private readonly RequestDelegate _next;  // RequestDelegate is automatically provided
    private readonly IInMemoryCacheService _cache;

    // Constructor now only receives the dependencies needed, including RequestDelegate
    public IdempotentMiddleware(RequestDelegate next, IInMemoryCacheService cache)
    {
        _next = next;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IdempotentAttribute>() is not null)
        {
            string cacheKey = await GenerateCacheKeyAsync(context);
            var cachedResponse = _cache.Get<CachedResponse>(cacheKey);
            if (cachedResponse != null)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = cachedResponse.StatusCode;
                await context.Response.WriteAsync(cachedResponse.Content);
                return;
            }

            var originalResponseBody = context.Response.Body;
            using var newResponseBody = new MemoryStream();
            context.Response.Body = newResponseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = new StreamReader(context.Response.Body).ReadToEnd();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var cacheEntry = new CachedResponse
            {
                StatusCode = context.Response.StatusCode,
                Content = responseText
            };

            _cache.Set(cacheKey, cacheEntry, 30);

            await newResponseBody.CopyToAsync(originalResponseBody);
        }
        else
        {
            await _next(context);
        }
    }

    private async Task<string> GenerateCacheKeyAsync(HttpContext context)
    {
        var request = context.Request;
        var keyBuilder = new StringBuilder();

        keyBuilder.Append(UserInfoHelper.GetUser().UserId);
        keyBuilder.Append(request.Path);
        keyBuilder.Append(request.Method);
        keyBuilder.Append(request.QueryString);

        context.Request.EnableBuffering();
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            var bodyString = await reader.ReadToEndAsync();
            keyBuilder.Append(bodyString);
            context.Request.Body.Position = 0;
        }

        return keyBuilder.ToString();
    }
}

public class CachedResponse
{
    public int StatusCode { get; set; }
    public string Content { get; set; } = string.Empty;
}