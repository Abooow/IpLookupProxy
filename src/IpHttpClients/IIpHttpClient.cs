using IpLookupProxy.Api.IpResponseModels;

namespace IpLookupProxy.Api.IpHttpClients;

public interface IIpHttpClient
{
    Task<IIpInfoModel> GetInfoAsync(string ipAddress);
}
