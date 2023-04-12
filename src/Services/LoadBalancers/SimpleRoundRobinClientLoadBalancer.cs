using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services.LoadBalancers;

internal sealed class SimpleRoundRobinClientLoadBalancer : IClientLoadBalancer
{
    private readonly ConfiguredClients _configuredClients;
    private int index = 0;

    public SimpleRoundRobinClientLoadBalancer(ConfiguredClients configuredClients)
    {
        _configuredClients = configuredClients;
    }

    public ClientConfigInfo GetClientConfig()
    {
        var enabledClients = _configuredClients.GetEnabledClients();
        if (!enabledClients.Any())
            throw new NoClientsFoundException($"All registered clients has been disabled.");

        int tries = enabledClients.Count();
        ClientConfigInfo? clientConfig = null;
        do
        {
            if (tries-- == 0)
                throw new AllClientsThrottledException();

            var client = GetClient(enabledClients);
            bool shouldThrottle = _configuredClients.ClientsRateLimiter.ShouldThrottleClient(client.Index, 1, out _);
            if (shouldThrottle)
                continue;

            clientConfig = client.Config;
        } while (clientConfig is null);

        return clientConfig;
    }

    private (ClientConfigInfo Config, int Index) GetClient(IEnumerable<(ClientConfigInfo Config, int Index)> enabledClients)
    {
        int i = index++ % enabledClients.Count();
        return enabledClients.First(x => x.Index == i);
    }
}
