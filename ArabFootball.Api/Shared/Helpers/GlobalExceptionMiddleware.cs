using ArabFootball.Shared.Helpers;
using System.Net;

namespace ArabFootball.Api.Shared.Helpers
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "حدث خطأ غير متوقع");

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<string>.Error(HttpStatusCode.InternalServerError, "حدث خطأ داخلي في السيرفر");

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
