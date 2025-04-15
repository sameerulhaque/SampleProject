using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleProject.Core.Exceptions
{
    //public class ExceptionFilter : IActionFilter
    //{
    //    public void OnActionExecuting(ActionExecutingContext context)
    //    {
    //    }

    //    public void OnActionExecuted(ActionExecutedContext context)
    //    {
    //        if (context.Result is BadRequestObjectResult badRequestResult)
    //        {
    //            string? errorMessage = null;
    //            if (badRequestResult.Value is string errorMessageString)
    //            {
    //                errorMessage = errorMessageString;
    //            }
    //            else if (badRequestResult.Value is APIResponseModel response)
    //            {
    //                errorMessage = response.StatusMessage;
    //            }
    //            else if (badRequestResult.Value is PagedAPIResponseModel responsePaged)
    //            {
    //                errorMessage = responsePaged.StatusMessage;
    //            }
    //            else
    //            {
    //                errorMessage = ((dynamic)(badRequestResult?.Value ?? new { error = "" })).error;
    //            }
    //            var httpContext = context.HttpContext;
    //            string payload = string.Empty;
    //            httpContext.Request.Body.Position = 0; // Reset the stream position
    //            using (var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true))
    //            {
    //                payload = reader.ReadToEndAsync().Result;
    //            }
    //            httpContext.Request.Body.Position = 0;
    //            if (errorMessage != null)
    //            {
    //                var QueryString = httpContext.Request.QueryString != null && httpContext.Request.QueryString.HasValue ? httpContext.Request.QueryString.Value : "";
    //                context.Result = new ObjectResult(new ErrorAPIResponseModel()
    //                {
    //                    Status = 400,
    //                    StatusMessage = errorMessage,
    //                    Detail = "error",
    //                    Instance = context.HttpContext.TraceIdentifier,
    //                    Payload = payload,
    //                    QueryString = QueryString
    //                })
    //                {
    //                    StatusCode = 400
    //                };

    //                var errorMessage1 = $"HTTP {context.HttpContext.GetEndpoint()?.ToString()} responded {400} error {errorMessage}" + ", UserId: " + UserInfoHelper.GetUser().UserId;
    //                Log.Error("Request failed: {ErrorMessage}, Payload: {Payload}, QueryString: {QueryString}",
    //                    errorMessage1, payload, context.HttpContext.Request.QueryString);
    //            }
    //        }
    //    }
    //}

}
