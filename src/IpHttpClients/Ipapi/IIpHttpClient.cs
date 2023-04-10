using IpLookupProxy.Api.Models;

namespace IpLookupProxy.Api.IpHttpClients.Ipapi;

public interface IIpHttpClient
{
    Task<IIpInfo> GetInfoAsync(string ipAddress);
}
