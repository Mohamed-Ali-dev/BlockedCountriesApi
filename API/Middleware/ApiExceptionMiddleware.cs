using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(
            RequestDelegate next,
            ILogger<ApiExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Handle successful responses with specific status codes (e.g., 429 Rate Limit)
                if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {
                    await HandleRateLimitResponse(context);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions from API calls
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError; // Default to 500

            // Customize status codes for specific exceptions
            if (exception is HttpRequestException httpEx)
            {
                statusCode = httpEx.StatusCode ?? HttpStatusCode.ServiceUnavailable;
            }

            context.Response.StatusCode = (int)statusCode;

            // Log the error
            _logger.LogError(exception, "API Error: {Message}", exception.Message);

            // Return a standardized error response
            var response = new
            {
                Status = statusCode,
                Message = "An error occurred while processing your request.",
                Details = exception.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleRateLimitResponse(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            // Extract Retry-After header if available
            var retryAfter = context.Response.Headers["Retry-After"].FirstOrDefault();

            var response = new
            {
                Status = context.Response.StatusCode,
                Message = "API rate limit exceeded.",
                RetryAfterSeconds = retryAfter
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
