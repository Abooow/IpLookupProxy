namespace IpLookupProxy.Api.Options;

public sealed class RateLimitRuleConfiguration
{
    public int Occurrences { get; set; }
    public TimeSpan TimeUnit { get; set; }
}
