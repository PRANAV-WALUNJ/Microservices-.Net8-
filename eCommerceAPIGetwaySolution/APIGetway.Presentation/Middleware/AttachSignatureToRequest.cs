namespace APIGetway.Presentation.Middleware
{
    public class AttachSignatureToRequest
    {
        private readonly RequestDelegate _next;
        public AttachSignatureToRequest(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.Headers["ApiGetway"] = "Signed";
            await _next(httpContext);
        }
    }
}
