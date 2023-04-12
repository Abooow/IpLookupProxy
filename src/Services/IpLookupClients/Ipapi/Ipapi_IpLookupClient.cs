using System.Text.Json;
using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Models;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services.IpLookupClients.Ipapi;

[IpLookupClientHandler(handlerName)]
internal class Ipapi_IpLookupClient : IIpLookupClient
{
    private const string handlerName = "ipapi";

    private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

    private readonly ILogger<Ipapi_IpLookupClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClientConfigInfo _clientConfigInfo;

    public Ipapi_IpLookupClient(ILogger<Ipapi_IpLookupClient> logger, IHttpClientFactory httpClientFactory, ClientConfigInfo clientConfigInfo)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _clientConfigInfo = clientConfigInfo;
    }

    public async Task<IIpInfo> GetIpInfoAsync(string ipAddress)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(GetApiEndpoint(ipAddress));
        string bodyString = await response.Content.ReadAsStringAsync();

        var badResponse = JsonSerializer.Deserialize<IpapiBadResponse>(bodyString, jsonSerializerOptions)!;
        if (badResponse.Success is not null && !badResponse.Success.Value)
        {
            _logger.LogCritical("An error occurred for client: {HandlerName} ({Name}) - StatusCode: {StatusCode} - Message: {Message}",
                handlerName, _clientConfigInfo.Name ?? "Unnamed", badResponse.Error!.Code, badResponse.Error!.Type);

            if (badResponse.Error.Code == 101)
                throw new BadClientApiKeyException(handlerName, _clientConfigInfo.Name);
            else
                throw new ClientErrorException(handlerName, badResponse.Error!.Type);
        }

        var ipResult = JsonSerializer.Deserialize<IpapiResponseModel>(bodyString, jsonSerializerOptions)!;

        if (!IpExists(ipResult))
            throw new IpDoesNotExistException(ipAddress);

        return ipResult;
    }

    private static bool IpExists(IpapiResponseModel ipResponseModel)
    {
        return ipResponseModel.CountryName is not null;
    }

    private string GetApiEndpoint(string ipAddress)
    {
        return $"http://api.ipapi.com/api/{ipAddress}?access_key={_clientConfigInfo.ApiKey}";
    }

    private class IpapiBadResponse
    {
        public bool? Success { get; set; }
        public IpapiErrorModel? Error { get; set; }
    }

    private class IpapiErrorModel
    {
        public int Code { get; set; }
        public string Type { get; set; } = default!;
    }
}
