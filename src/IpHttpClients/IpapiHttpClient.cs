using IpLookupProxy.Api.IpResponseModels;
using IpLookupProxy.Api.Models;

namespace IpLookupProxy.Api.IpHttpClients;

internal class IpapiHttpClient : IIpHttpClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClientsConfiguration _clientsConfiguration;

    public IpapiHttpClient(IHttpClientFactory httpClientFactory, ClientsConfiguration clientsConfiguration)
    {
        _httpClientFactory = httpClientFactory;
        _clientsConfiguration = clientsConfiguration;
    }

    public async Task<IIpInfoModel> GetInfoAsync(string ipAddress)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var result = await httpClient.GetFromJsonAsync<IpapiResponseModel>(GetApiEndpoint(ipAddress));

        return result!;
    }

    private string GetApiEndpoint(string ipAddress)
    {
        return $"http://api.ipapi.com/api/{ipAddress}?access_key={_clientsConfiguration.GetApiKey("ipapi")}";
    }
}
