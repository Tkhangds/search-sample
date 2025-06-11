using System.Net;

namespace Eme_Search.Utils;

public class SuccessResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public required string Message { get; set; }
    public object? Data { get; set; }
}