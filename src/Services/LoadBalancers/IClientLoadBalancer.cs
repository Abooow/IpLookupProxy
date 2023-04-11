using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services.LoadBalancers;

public interface IClientLoadBalancer
{
    ClientConfigInfo GetClientConfig();
}
