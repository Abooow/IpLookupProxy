namespace IpLookupProxy.Api.Options;

public sealed class ClientConfigInfo
{
    public string Handler { get; set; } = default!;
    public string? Name { get; set; }
    public string? Site { get; set; }
    public bool Enabled { get; set; } = true;
    public string? ApiKey { get; set; }
    public RateLimitRuleConfiguration[] RateLimitingRules { get; set; } = default!;
}

