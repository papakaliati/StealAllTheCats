using System.Text.Json;

namespace StealAllTheCats.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred.");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            string result = JsonSerializer.Serialize(new { 
                message = $"An unexpected error occurred. Please try again later.",
            });
            await context.Response.WriteAsync(result);
        }
    }
}
