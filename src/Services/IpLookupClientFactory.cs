using IpLookupProxy.Api.Options;
using IpLookupProxy.Api.Services.IpLookupClients;
using IpLookupProxy.Api.Services.IpLookupClients.Ipapi;
using IpLookupProxy.Api.Services.LoadBalancers;

namespace IpLookupProxy.Api.Services;

internal class IpLookupClientFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IClientLoadBalancer _clientLoadBalancer;

    public IpLookupClientFactory(IServiceProvider serviceProvider, IClientLoadBalancer clientLoadBalancer)
    {
        _serviceProvider = serviceProvider;
        _clientLoadBalancer = clientLoadBalancer;
    }

    public (IIpLookupClient Client, ClientConfigInfo configInfo) GetIpLookupClient()
    {
        var clientConfigInfo = _clientLoadBalancer.GetClientConfig();
        var httpClientFactory = _serviceProvider.GetRequiredService<IHttpClientFactory>();

        return clientConfigInfo.Handler switch
        {
            "ipapi" => (new Ipapi_IpLookupClient(_serviceProvider.GetRequiredService<ILogger<Ipapi_IpLookupClient>>(), httpClientFactory, clientConfigInfo), clientConfigInfo),
            _ => throw new Exception($"'{clientConfigInfo.Handler}' is not a registered client."),
        };
    }
}
