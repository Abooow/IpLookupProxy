using IpLookupProxy.Api.Options;
using IpLookupProxy.Api.Services.IpLookupClients;
using IpLookupProxy.Api.Services.IpLookupClients.Ipapi;

namespace IpLookupProxy.Api.Services;

internal class IpLookupClientFactory
{
    private readonly IServiceProvider _serviceProvider;

    public IpLookupClientFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IIpLookupClient GetIpHttpClient(string clientName)
    {
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
        var clientsConfiguration = _serviceProvider.GetRequiredService<ClientsConfiguration>();
        return clientName switch
        {
            "ipapi" => new Ipapi_IpLookupClient(_serviceProvider.GetRequiredService<ILogger<Ipapi_IpLookupClient>>(), httpClientFactory, clientsConfiguration),
            _ => throw new Exception($"'{clientName}' is not a registered client."),
        };
    }
}
