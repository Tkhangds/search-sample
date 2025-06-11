using Eme_Search.Constant;

namespace Eme_Search.Exceptions;

public class UnprocessableRequestException : Exception
{
    public UnprocessableRequestException(string message, string[]? errors = null,
        string? exceptionCode = ExceptionConvention.UnprocessableRequest) : base(message)
    {
        ExceptionCode = exceptionCode;
        Errors = errors;
    }

    public string? ExceptionCode { get; }

    public string[]? Errors { get; set; }
}