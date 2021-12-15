namespace IpLookupProxy.Api;

internal class TokenBucketRateLimiter
{
    private static readonly object syncRoot = new object();

    private readonly long bucketTokenCapacity;
    private readonly long ticksRefillInterval;
    private long nextRefillTime;

    private long tokens;

    public long CurrentTokenCount
    {
        get
        {
            lock (syncRoot)
            {
                UpdateTokens();
                return tokens;
            }
        }
    }

    public TokenBucketRateLimiter(long bucketTokenCapacity, TimeSpan refillInterval)
    {
        if (bucketTokenCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(bucketTokenCapacity), "bucket token capacity can not be negative");

        this.bucketTokenCapacity = bucketTokenCapacity;
        ticksRefillInterval = refillInterval.Ticks;
    }

    public bool ShouldThrottle(long tokens, out TimeSpan waitTime)
    {
        if (tokens <= 0)
            throw new ArgumentOutOfRangeException(nameof(tokens), "Should be positive integer");

        lock (syncRoot)
        {
            UpdateTokens();
            if (this.tokens < tokens)
            {
                var timeToIntervalEnd = nextRefillTime - DateTime.UtcNow.Ticks;
                if (timeToIntervalEnd < 0)
                    return ShouldThrottle(tokens, out waitTime);

                waitTime = TimeSpan.FromTicks(timeToIntervalEnd);
                return true;
            }
            this.tokens -= tokens;

            waitTime = TimeSpan.Zero;
            return false;
        }
    }

    private void UpdateTokens()
    {
        var currentTime = DateTime.UtcNow.Ticks;

        if (currentTime < nextRefillTime)
            return;

        tokens = bucketTokenCapacity;
        nextRefillTime = currentTime + ticksRefillInterval;
    }
}
