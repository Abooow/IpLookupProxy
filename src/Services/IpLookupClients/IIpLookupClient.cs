using IpLookupProxy.Api.Models;

namespace IpLookupProxy.Api.Services.IpLookupClients;

public interface IIpLookupClient
{
    string HandlerName { get; }
    Task<IIpInfo> GetIpInfoAsync(string ipAddress);
}
