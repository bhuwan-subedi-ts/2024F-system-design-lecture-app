using System.Net;
using System.Text.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }catch(Exception ex)
        {
            _logger.LogError(ex,
            "Unhandled exception occurred at {Path}, Method: {Method}",
            context.Request.Path,
            context.Request.Method);
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
              Message = "An unexpected error occured. Please try again later."  
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}