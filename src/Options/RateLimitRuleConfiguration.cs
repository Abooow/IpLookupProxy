namespace IpLookupProxy.Api.Options;

public class RateLimitRuleConfiguration
{
    public int Occurrences { get; set; }
    public TimeSpan TimeUnit { get; set; }
}
