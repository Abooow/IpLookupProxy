using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace IpLookupProxy.Api.Models;

public class TooManyRequestsResult : IResult
{
    private readonly object? _responseObject;

    public TooManyRequestsResult()
    {
    }

    public TooManyRequestsResult(object responseObject)
    {
        _responseObject = responseObject;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;

        if (_responseObject is not null)
        {
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(_responseObject));
        }
    }
}
