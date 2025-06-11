using Eme_Search.Constant;

namespace Eme_Search.Exceptions;

[Serializable]
public class BadRequestException : Exception
{
    public BadRequestException(string message, string? exceptionCode = ExceptionConvention.BadRequest) : base(message)
    {
        ExceptionCode = exceptionCode;
    }

    public string? ExceptionCode { get; }
}