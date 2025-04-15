using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SampleProject.Infrastructure.Authentication;
using SampleProject.Infrastructure.Exceptions;
using SampleProject.Shared.Models.Misc;
using Serilog;
using System.Diagnostics;
using System.Text.Json;
using static SampleProject.Shared.Models.Misc.APIResponseModel;

namespace SampleProject.Core.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                // Log the request before calling the next middleware
                string payload = string.Empty;
                context.Request.EnableBuffering();
                context.Request.Body.Position = 0; // Reset the stream position
                using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
                {
                    payload = await reader.ReadToEndAsync();  // Use await here instead of .Result
                }
                context.Request.Body.Position = 0;
                var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";

                // Log the request details, including payload
                Log.Information("HTTP {RequestMethod} {RequestPath} started with QueryString: {QueryString}, Payload: {Payload}, UserId: {UserId}",
                    context.Request.Method,
                    context.Request.Path,
                    queryString,
                    payload,
                    UserInfoHelper.GetUser().UserId);

                // Continue with the request pipeline
                await _next(context);

                // Log after the response is returned
                sw.Stop();
                Log.Information("HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms with Payload: {Payload}, QueryString: {QueryString}, UserId: {UserId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.Elapsed.TotalMilliseconds,
                    payload,
                    queryString + ", UserId: " + UserInfoHelper.GetUser().UserId);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await HandleExceptionAsync(context, sw, ex);

                // Log exceptions as "Error"
                Log.Error(ex, "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms",
                    context.Request.Method, context.Request.Path, 500, sw.Elapsed.TotalMilliseconds);
            }
        }

        public static Task HandleExceptionAsync(HttpContext context, Stopwatch sw, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var result = new APIResponseModel();
            if (ex is BadRequestException badRequestEx)
            {
                result.BadRequest(badRequestEx.Error, badRequestEx.Errors);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
            else if (ex is UnauthorizedException unauthorizedEx)
            {
                result.Unauthorized(unauthorizedEx.Error);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            else if (ex is ForbiddenException forbiddenEx)
            {
                result.Forbidden(forbiddenEx.Error);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
            }
            else if (ex is NotFoundException notFoundEx)
            {
                result.NotFound(notFoundEx.Error);
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
            else if (ex is MethodNotAllowedException methodNotAllowedEx)
            {
                result.MethodNotAllowed(methodNotAllowedEx.Error);
                context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
            }
            else if (ex is ConflictException conflictEx)
            {
                result.Conflict(conflictEx.Error);
                context.Response.StatusCode = StatusCodes.Status409Conflict;
            }
            else if (ex is TooManyRequestException tooManyRequestEx)
            {
                result.TooManyRequest(tooManyRequestEx.Error);
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            }
            else if (ex is Infrastructure.Exceptions.NotImplementedException notImplementedEx)
            {
                result.NotImplemented(notImplementedEx.Error);
                context.Response.StatusCode = StatusCodes.Status501NotImplemented;
            }
            else if (ex is Exception generalEx)
            {
                result.InternalServerError(generalEx.Message);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            Log.Error(ex, "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.Elapsed.TotalMilliseconds);

            return context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }
    }
}
