using IpLookupProxy.Api.Models;
using Microsoft.Extensions.Options;
using System.Net.Mime;
using System.Text.Json;

namespace IpLookupProxy.Api.Middlewares;

public class ApiServerKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiServerKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IOptions<ApiServerKeySettings> apiServerKeyOptions)
    {
        var apiServerKeySettings = apiServerKeyOptions.Value;

        string validKey = apiServerKeySettings.Key;
        bool hasQueryStringKey = httpContext.Request.Query.TryGetValue(apiServerKeySettings.QueryName, out var queryStringKeys);

        if (!hasQueryStringKey || queryStringKeys.Count > 1)
        {
            await WriteBadResponseAsync(httpContext.Response, "A key is required.");
            return;
        }

        if (queryStringKeys.First() != validKey)
        {
            await WriteBadResponseAsync(httpContext.Response, "Not a valid key.");
            return;
        }

        await _next(httpContext);
    }

    private static Task WriteBadResponseAsync(HttpResponse response, string message)
    {
        response.StatusCode = StatusCodes.Status400BadRequest;
        response.ContentType = MediaTypeNames.Application.Json;

        return response.WriteAsync(JsonSerializer.Serialize(new { message }));
    }
}

public static class ApiServerKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseApiServerKey(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiServerKeyMiddleware>();
    }
}