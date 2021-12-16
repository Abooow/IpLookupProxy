namespace IpLookupProxy.Api.Models;

internal class ClientConfigInfo
{
    public string Name { get; set; }
    public string ApiKey { get; set; }
    public RateLimitRuleConfiguration[] RateLimitingRules { get; set; }
}
