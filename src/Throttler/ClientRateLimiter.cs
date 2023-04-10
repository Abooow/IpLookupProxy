using System.Collections.Concurrent;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api;

internal class ClientRateLimiter
{
    private readonly ConcurrentDictionary<string, RateLimiter> _clients;

    public ClientRateLimiter(IEnumerable<ClientConfigInfo> clientConfigInfos)
        : this(clientConfigInfos.Select(x => new KeyValuePair<string, IEnumerable<RateLimitRule>>(
            x.Name!,
            x.RateLimitingRules!.Select(y => new RateLimitRule(y.Occurrences, y.TimeUnit)))))
    {
    }

    public ClientRateLimiter(params KeyValuePair<string, IEnumerable<RateLimitRule>>[] clients)
        : this(clients.AsEnumerable())
    {
    }

    public ClientRateLimiter(IEnumerable<KeyValuePair<string, IEnumerable<RateLimitRule>>> clients)
    {
        _clients = new(clients.Select(x => new KeyValuePair<string, RateLimiter>(x.Key, new(x.Value))));
    }

    public IEnumerable<string> GetClients()
    {
        return _clients.Keys;
    }

    public bool ShouldThrottleClient(string client, long tokens, out TimeSpan waitTime)
    {
        if (!_clients.TryGetValue(client, out RateLimiter? rateLimiter))
            throw new KeyNotFoundException($"Key client '{client}' was not found.");

        return rateLimiter.ShouldThrottle(tokens, out waitTime);
    }
}
