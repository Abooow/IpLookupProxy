namespace IpLookupProxy.Api;

internal sealed record RateLimitRule(int Occurrences, TimeSpan TimeUnit);
