using Eme_Search.Constant;

namespace Eme_Search.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message, string? exceptionCode = ExceptionConvention.BadRequest) : base(message)
    {
        ExceptionCode = exceptionCode;
    }

    public string? ExceptionCode { get; }
}