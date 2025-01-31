using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate requestDelegate)
    {
        public async Task InvokeAsync(HttpContext httpContext)
        {
            string message = "Internal server error,Please try again";
            int statusCode=(int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await requestDelegate(httpContext);
                if(httpContext.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    title = "Warning";
                    message = "To many request";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    await ModifyHeader(httpContext, title, message,statusCode);
                }

                if (httpContext.Response.StatusCode == StatusCodes.Status401Unauthorized)   
                {
                    title = "Alerrt";
                    message = "You are not authorize";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    await ModifyHeader(httpContext, title, message, statusCode);
                }

                if (httpContext.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    message = "You are not allowed to access this";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    await ModifyHeader(httpContext, title, message, statusCode);
                }
            }
            catch (Exception ex)
            {

                LogException.LogExceptions(ex);

                if (ex is TimeoutException ||  ex is TaskCanceledException)
                {
                    title = "Out of Time";
                    message = "Request time outs";
                    statusCode = (int)StatusCodes.Status408RequestTimeout;
                    await ModifyHeader(httpContext, title, message, statusCode);
                }
            }
        }
        private static async Task ModifyHeader(HttpContext httpContext, string title, string message, int statusCode)
        {
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail =message,Status = statusCode,Title = title
            }),CancellationToken.None);
        }
    }
}
