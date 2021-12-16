using IpLookupProxy.Api.IpHttpClients;
using IpLookupProxy.Api.Models;

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
        return clientName switch
        {
            "ipapi" => new IpapiHttpClient(_serviceProvider.GetRequiredService<IHttpClientFactory>(), _serviceProvider.GetRequiredService<ClientsConfiguration>()),
            _ => throw new Exception($"'{clientName}' is not a registered client."),
        };
    }
}
