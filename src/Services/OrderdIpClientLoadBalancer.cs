using IpLookupProxy.Api.Exceptions;

namespace IpLookupProxy.Api.Services;

internal sealed class OrderdIpClientLoadBalancer : IIpClientLoadBalancer
{
    private readonly ClientRateLimiter _clientRateLimiter;

    public OrderdIpClientLoadBalancer(ClientRateLimiter clientRateLimiter)
    {
        _clientRateLimiter = clientRateLimiter;
    }

    public string GetClient()
    {
        var clients = _clientRateLimiter.GetClients();
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
