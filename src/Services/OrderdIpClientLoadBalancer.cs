using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Models;

namespace IpLookupProxy.Api.Services;

internal sealed class OrderdIpClientLoadBalancer : IIpClientLoadBalancer
{
    private readonly ClientsConfiguration _clientsConfiguration;
    private readonly ClientRateLimiter _clientRateLimiter;

    public OrderdIpClientLoadBalancer(ClientsConfiguration clientsConfiguration, ClientRateLimiter clientRateLimiter)
    {
        _clientsConfiguration = clientsConfiguration;
        _clientRateLimiter = clientRateLimiter;
    }

    public string GetClient()
    {
        var clients = _clientsConfiguration.GetEnabledClients();
        if (!clients.Any())
            throw new NoClientsFoundException($"All registered clients has been disabled.");

        foreach (var client in clients)
        {
            bool shouldThrottle = _clientRateLimiter.ShouldThrottleClient(client, 1, out _);
            if (shouldThrottle)
                continue;

            return client;
        }

        throw new AllClientsThrottledException();
    }
}
