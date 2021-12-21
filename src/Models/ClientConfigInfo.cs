namespace IpLookupProxy.Api.Models;

internal class ClientConfigInfo
{
    public string Name { get; set; } = default!;
    public string Site { get; set; } = default!;
    public bool Enabled { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public RateLimitRuleConfiguration[] RateLimitingRules { get; set; } = default!;
}
