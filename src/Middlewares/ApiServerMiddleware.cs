using System.Net.Mime;
using System.Text.Json;
using IpLookupProxy.Api.Options;
using Microsoft.Extensions.Options;

namespace IpLookupProxy.Api.Middlewares;

public class ApiServerMiddleware
{
    private readonly RequestDelegate _next;

    public ApiServerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, IOptions<ApiServerSettings> apiServerOptions, ILogger<ApiServerMiddleware> logger)
    {
        var apiServerKeySettings = apiServerOptions.Value;

        if (!await ValidateRemoteConnectionAsync(httpContext, apiServerKeySettings, logger))
            return;

        if (!await ValidateApiKeyAsync(httpContext, apiServerKeySettings))
            return;

        await _next(httpContext);
    }

    private static async Task<bool> ValidateRemoteConnectionAsync(HttpContext httpContext, ApiServerSettings apiServerSettings, ILogger<ApiServerMiddleware> logger)
    {
        if (apiServerSettings.AllowAnyRemote)
            return true;

        string remoteIp = httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "0.0.0.1";
        if (remoteIp == "0.0.0.1")
            return true;

        bool allowRemoteHost = apiServerSettings.AllowedRemotes.Any(x => x == remoteIp);
        if (!allowRemoteHost)
        {
            logger.LogInformation("Blocked request from {RemoteIp}", remoteIp);
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
        bool hasQueryStringKey = httpContext.Request.Query.TryGetValue(apiServerSettings.QueryName, out var queryStringKeys);

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