namespace IpLookupProxy.Api;

internal class RateLimiter
{
    private readonly TokenBucketRateLimiter[] _tokenBuckets;

    public RateLimiter(params RateLimitRule[] rateLimitRules)
        : this(rateLimitRules.AsEnumerable())
    {
    }

    public RateLimiter(IEnumerable<RateLimitRule> rateLimitRules)
    {
        _tokenBuckets = rateLimitRules.Select(x => new TokenBucketRateLimiter(x.Occurrences, x.TimeUnit)).ToArray();
    }

    public bool ShouldThrottle(long tokens, out TimeSpan waitTime)
    {
        bool shouldThrottle = false;
        waitTime = TimeSpan.Zero;

        for (int i = 0; i < _tokenBuckets.Length; i++)
        {
            shouldThrottle |= _tokenBuckets[i].ShouldThrottle(tokens, out TimeSpan bucketWaitTime);
            waitTime += bucketWaitTime;
        }

        return shouldThrottle;
    }
}
