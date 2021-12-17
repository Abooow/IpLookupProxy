namespace IpLookupProxy.Api.Models;

public class ApiServerKeySettings
{
    public bool RequireKey { get; set; }
    public string QueryName { get; set; }
    public string Key { get; set; }
}
