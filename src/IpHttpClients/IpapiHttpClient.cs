using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.IpResponseModels;
using IpLookupProxy.Api.Models;
using System.Text.Json;

namespace IpLookupProxy.Api.IpHttpClients;

internal class IpapiHttpClient : IIpHttpClient
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

    private readonly ILogger<IpapiHttpClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClientsConfiguration _clientsConfiguration;

    public IpapiHttpClient(ILogger<IpapiHttpClient> logger, IHttpClientFactory httpClientFactory, ClientsConfiguration clientsConfiguration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _clientsConfiguration = clientsConfiguration;
    }

    public async Task<IIpInfoModel> GetInfoAsync(string ipAddress)
    {
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.GetAsync(GetApiEndpoint(ipAddress));
        string bodyString = await response.Content.ReadAsStringAsync();

        var badResponse = JsonSerializer.Deserialize<IpapiBadResponse>(bodyString, jsonSerializerOptions)!;
        if (badResponse.Success is not null && !badResponse.Success.Value)
        {
            _logger.LogCritical("An error occurred for client: {ClientName} - StatusCode: {StatusCode} - Message: {Message}",
                "ipapi", badResponse.Error!.Code, badResponse.Error!.Type);
            throw new BadClientApiKeyException("ipapi", badResponse.Error!.Type);
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
        return $"http://api.ipapi.com/api/{ipAddress}?access_key={_clientsConfiguration.GetApiKey("ipapi")}";
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
