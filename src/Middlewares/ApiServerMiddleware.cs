using IpLookupProxy.Api.Models;
using Microsoft.Extensions.Options;
using System.Net.Mime;
using System.Text.Json;

namespace IpLookupProxy.Api.Middlewares;

public class ApiServerMiddleware
{
    private readonly RequestDelegate _next;

    public ApiServerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IOptions<ApiServerSettings> apiServerOptions)
    {
        var apiServerKeySettings = apiServerOptions.Value;

        if (!await ValidateRemoteConnectionAsync(httpContext, apiServerKeySettings))
            return;

        if (!await ValidateApiKeyAsync(httpContext, apiServerKeySettings))
            return;

        await _next(httpContext);
    }

    private static async Task<bool> ValidateRemoteConnectionAsync(HttpContext httpContext, ApiServerSettings apiServerSettings
    {
        if (apiServerSettings.AllowAnyRemote)
            return true;

        string remoteIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "::1";
        if (remoteIp == "::1")
            return true;

        bool allowRemoteHost = apiServerSettings.AllowedRemotes?.Any(x => x == remoteIp) ?? true;

        if (!allowRemoteHost)
        {
            await WriteBadResponseAsync(httpContext.Response, "Not allowed.");
            return false;
        }

        return true;
    }

    private static async Task<bool> ValidateApiKeyAsync(HttpContext httpContext, ApiServerSettings apiServerSettings)
    {
        if (!apiServerSettings.RequireKey)
            return true;

        string? validKey = apiServerSettings.Key;
        bool hasQueryStringKey = httpContext.Request.Query.TryGetValue(apiServerSettings?.QueryName ?? "key", out var queryStringKeys);

        if (!hasQueryStringKey || queryStringKeys.Count > 1)
        {
            await WriteBadResponseAsync(httpContext.Response, "A key is required.");
            return false;
        }

        if (queryStringKeys.First() != validKey)
        {
            await WriteBadResponseAsync(httpContext.Response, "Not a valid key.");
            return false;
        }

        return true;
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
        return builder.UseMiddleware<ApiServerMiddleware>();
    }
}