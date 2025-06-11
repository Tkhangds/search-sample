using Eme_Search.Constant;

namespace Eme_Search.Exceptions;

[Serializable]
public class ForbiddenException : Exception
{
    public ForbiddenException(string message, string? exceptionCode = ExceptionConvention.Forbidden) : base(message)
    {
        ExceptionCode = exceptionCode;
    }

    public string? ExceptionCode { get; }
}