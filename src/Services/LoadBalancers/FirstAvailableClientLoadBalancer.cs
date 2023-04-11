using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services.LoadBalancers;

internal sealed class FirstAvailableClientLoadBalancer : IClientLoadBalancer
{
    private readonly ConfiguredClients _configuredClients;

    public FirstAvailableClientLoadBalancer(ConfiguredClients configuredClients)
    {
        _configuredClients = configuredClients;
    }

    public ClientConfigInfo GetClientConfig()
    {
        var enabledClients = _configuredClients.GetEnabledClients();
        if (!enabledClients.Any())
            throw new NoClientsFoundException($"All registered clients has been disabled.");

        foreach (var client in enabledClients)
        {
            bool shouldThrottle = _configuredClients.ClientsRateLimiter.ShouldThrottleClient(client.Index, 1, out _);
            if (shouldThrottle)
                continue;

            return client.Config;
        }

        throw new AllClientsThrottledException();
    }
}
