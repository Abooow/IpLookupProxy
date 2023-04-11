using System.Collections.ObjectModel;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services;

internal class ClientsRateLimiter
{
    private readonly ReadOnlyDictionary<string, RateLimiter> _clients;

    public ClientsRateLimiter(IEnumerable<ClientConfigInfo> clientsConfigInfo)
    {
        var items = clientsConfigInfo
            .Select(x => new KeyValuePair<string, RateLimiter>(x.Name, GetRateLimiter(x.RateLimitingRules)));

        _clients = new(new Dictionary<string, RateLimiter>(items));
    }

    private static RateLimiter GetRateLimiter(RateLimitRuleConfiguration[] rateLimitingRules)
    {
        return new RateLimiter(rateLimitingRules.Select(x => new RateLimitRule(x.Occurrences, x.TimeUnit)));
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
