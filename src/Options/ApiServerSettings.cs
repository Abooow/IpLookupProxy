namespace IpLookupProxy.Api.Options;

public class ApiServerSettings
{
    public bool RequireKey { get; set; }
    public string QueryName { get; set; } = "key";
    public string? Key { get; set; }
    public bool AllowAnyRemote { get; set; } = true;
    public string[] AllowedRemotes { get; set; } = Array.Empty<string>();
}
