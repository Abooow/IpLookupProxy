using IpLookupProxy.Api.Exceptions;

namespace IpLookupProxy.Api.Services;

internal sealed class FirstAvailableIpLookupClientLoadBalancer : IIpLookupClientLoadBalancer
{
    private readonly ConfiguredClients _configuredClients;

    public FirstAvailableIpLookupClientLoadBalancer(ConfiguredClients configuredClients)
    {
        _configuredClients = configuredClients;
    }

    public string GetClient()
    {
        var enabledClients = _configuredClients.GetEnabledClients();
        if (!enabledClients.Any())
            throw new NoClientsFoundException($"All registered clients has been disabled.");

        foreach (var client in enabledClients)
        {
            bool shouldThrottle = _configuredClients.ClientsRateLimiter.ShouldThrottleClient(client, 1, out _);
            if (shouldThrottle)
                continue;

            return client;
        }

        throw new AllClientsThrottledException();
    }
}
