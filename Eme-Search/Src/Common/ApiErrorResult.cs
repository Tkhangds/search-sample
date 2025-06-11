namespace Eme_Search.Common;

public class ApiErrorResult
{
    public ApiErrorResult()
    {
    }

    public ApiErrorResult(int statusCode, string message, string exceptionCode, IEnumerable<string> errors)
    {
        StatusCode = statusCode;
        Message = message;
        ExceptionCode = exceptionCode;
        Errors = errors;
    }

    public int StatusCode { get; set; }

    public string Message { get; set; }

    public IEnumerable<string> Errors { get; set; }

    public string ExceptionCode { get; set; }
}