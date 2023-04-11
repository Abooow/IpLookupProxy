using IpLookupProxy.Api.Models;

namespace IpLookupProxy.Api.Services.IpLookupClients;

public interface IIpLookupClient
{
    string ClientName { get; }
    Task<IIpInfo> GetIpInfoAsync(string ipAddress);
}
