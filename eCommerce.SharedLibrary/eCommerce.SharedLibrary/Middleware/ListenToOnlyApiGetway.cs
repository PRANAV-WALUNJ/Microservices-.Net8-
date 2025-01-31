using Microsoft.AspNetCore.Http;

namespace eCommerce.SharedLibrary.Middleware
{
    public class ListenToOnlyApiGetway(RequestDelegate requestDelegate)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var sinedHeader = context.Request.Headers["Api-Getway"];

            if(sinedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode =StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service unavailable");
                return;
            }
            else
            {
                await requestDelegate(context);
            }
        }
    }
}
