using IpLookupProxy.Api.Models;

namespace IpLookupProxy.Api.Services.IpLookupClients;

public interface IIpLookupClient
{
    Task<IIpInfo> GetIpInfoAsync(string ipAddress);
}
