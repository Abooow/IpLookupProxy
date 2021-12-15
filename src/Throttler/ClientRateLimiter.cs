using System.Collections.Concurrent;

namespace IpLookupProxy.Api;

internal class ClientRateLimiter
{
    private readonly ConcurrentDictionary<string, RateLimiter> _clients;

    public ClientRateLimiter(params KeyValuePair<string, RateLimitRule[]>[] clients)
    {
        _clients = new(clients.Select(x => new KeyValuePair<string, RateLimiter>(x.Key, new(x.Value))));
    }

    public bool ShouldThrottleClient(string client, long tokens, out TimeSpan waitTime)
    {
        if (!_clients.TryGetValue(client, out RateLimiter? rateLimiter))
            throw new KeyNotFoundException($"Key client '{client}' was not found.");

        return rateLimiter.ShouldThrottle(tokens, out waitTime);
    }
}
