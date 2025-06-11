using System.Net;
using Eme_Search.Constant;
using Eme_Search.Utils;
using Microsoft.AspNetCore.Diagnostics;

namespace Eme_Search.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        await HandleExceptionAsync(httpContext, exception, cancellationToken);
        
        return true;
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ExceptionResponse exceptionResponse)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)exceptionResponse.StatusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                exceptionResponse.StatusCode,
                exceptionResponse.Message,
                exceptionResponse.ExceptionCode,
                exceptionResponse.Errors
            }, cancellationToken);
        }
        else
        {
            var response = exception switch
            {
                KeyNotFoundException _ => new ExceptionResponse(HttpStatusCode.NotFound, exception.Message,
                    ExceptionConvention.NotFound),
                UnauthorizedAccessException _ => new ExceptionResponse(HttpStatusCode.Unauthorized, "Unauthorized.",
                    ExceptionConvention.Unauthorized),
                
                _ => new ExceptionResponse(HttpStatusCode.InternalServerError,
                    "Internal server error. Please retry later.", ExceptionConvention.InternalServerError)
            };

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "An unexpected error occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)response.StatusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                response.StatusCode,
                response.Message,
                response.ExceptionCode,
                response.Errors
            }, cancellationToken);
        }
    }
}