using System.Net;

namespace Eme_Search.Utils;

public class ExceptionResponse : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    
    public string ExceptionCode { get; set; }
    
    public string[]? Errors { get; set; }
    
    public ExceptionResponse(HttpStatusCode statusCode, string message, string exceptionCode, string[]? errors = null) : base(message)
    {
        StatusCode = statusCode;
        ExceptionCode = exceptionCode;
        Errors = errors;
    }
}