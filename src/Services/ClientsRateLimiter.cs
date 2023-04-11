using System.Collections.ObjectModel;
using IpLookupProxy.Api.Options;

namespace IpLookupProxy.Api.Services;

internal class ClientsRateLimiter
{
    private readonly IReadOnlyList<RateLimiter> _clientsRateLimiters;

    public ClientsRateLimiter(IEnumerable<ClientConfigInfo> clientsConfigInfo)
    {
        var clientRateLimiters = clientsConfigInfo
            .Select(x => GetRateLimiter(x.RateLimitingRules))
            .ToList();

        _clientsRateLimiters = new ReadOnlyCollection<RateLimiter>(clientRateLimiters);
    }

    private static RateLimiter GetRateLimiter(RateLimitRuleConfiguration[] rateLimitingRules)
    {
        return new RateLimiter(rateLimitingRules.Select(x => new RateLimitRule(x.Occurrences, x.TimeUnit)));
    }

    public bool ShouldThrottleClient(int clientIndex, long tokens, out TimeSpan waitTime)
    {
        if (clientIndex < 0 || clientIndex > _clientsRateLimiters.Count)
            throw new IndexOutOfRangeException($"Could not find a configured client at index '{clientIndex}'.");

        return _clientsRateLimiters[clientIndex].ShouldThrottle(tokens, out waitTime);
    }
}
