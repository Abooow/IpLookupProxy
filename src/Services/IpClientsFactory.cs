using IpLookupProxy.Api.IpHttpClients.Ipapi;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services;

internal class IpClientsFactory
{
    private readonly IServiceProvider _serviceProvider;

    public IpClientsFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IIpHttpClient GetIpHttpClient(string clientName)
    {
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();
        var clientsConfiguration = _serviceProvider.GetRequiredService<ClientsConfiguration>();
        return clientName switch
        {
            "ipapi" => new IpapiHttpClient(_serviceProvider.GetRequiredService<ILogger<IpapiHttpClient>>(), httpClientFactory, clientsConfiguration),
            _ => throw new Exception($"'{clientName}' is not a registered client."),
        };
    }
}
