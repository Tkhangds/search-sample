using System.Net;

using Eme_Search.Common;
using Eme_Search.Constant;
using Eme_Search.Utils;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace Eme_Search.Exceptions;

public class ExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        await HandleException(httpContext, exception, cancellationToken);

        return true;
    }

    private Task HandleException(HttpContext context, Exception ex, CancellationToken cancellationToken)
    {
        var (statusCode, message, exceptionCode, errors) = ex switch
        {
            KeyNotFoundException _ => (StatusCodes.Status404NotFound, ex.Message, ExceptionConvention.NotFound, null),
            ResourceNotFoundException _ => (StatusCodes.Status404NotFound, ex.Message, ExceptionConvention.NotFound, null),
            UnauthorizedAccessException _ => (StatusCodes.Status401Unauthorized, "Unauthorized.",
                ExceptionConvention.Unauthorized, null),
            ForbiddenException _ => (StatusCodes.Status403Forbidden, "Forbidden.", ExceptionConvention.Forbidden, null),
            BadRequestException exception => (StatusCodes.Status400BadRequest, ex.Message,
                exception.ExceptionCode ?? ExceptionConvention.BadRequest, null),
            UnprocessableRequestException exception => (StatusCodes.Status422UnprocessableEntity, ex.Message,
                exception.ExceptionCode ?? ExceptionConvention.UnprocessableRequest, exception.Errors),

            _ => (StatusCodes.Status500InternalServerError, "Internal server error. Please retry later.",
                ExceptionConvention.InternalServerError, null)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            _logger.LogError(ex, "An unexpected error occurred.");


        var result = JsonConvert.SerializeObject(new ApiErrorResult(statusCode,
            message, exceptionCode, errors));

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        return context.Response.WriteAsync(result, cancellationToken);
    }
}
