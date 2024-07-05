using Model;
using Newtonsoft.Json;
using System.Net;

namespace TodosApi.Middleware
{
   public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomMiddleware> _logger;

        public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                _logger.LogInformation("Handling request: " + httpContext.Request.Method + " " + httpContext.Request.Path);

                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
            _logger.LogInformation("Finished handling request.");
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            ApiResponse response;

            switch (ex)
            {
                case UnauthorizedAccessException _:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = new ApiResponse
                    {
                        Code = context.Response.StatusCode,
                        Message = "Unauthorized",
                        Data = ex.Message
                    };
                    break;
                case BadHttpRequestException _:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new ApiResponse
                    {
                        Code = context.Response.StatusCode,
                        Message = "BadRequest",
                        Data = ex
                    };
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new ApiResponse
                    {
                        Code = context.Response.StatusCode,
                        Message = "InternalServerError",
                        Data = ex.Message
                    };
                    break;
            }

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
