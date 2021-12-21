namespace IpLookupProxy.Api.Models;

public class ApiServerSettings
{
    public bool RequireKey { get; set; }
    public string? QueryName { get; set; }
    public string? Key { get; set; }
    public bool AllowAnyRemote { get; set; }
    public string[]? AllowedRemotes { get; set; }
}
