using IpLookupProxy.Api.DataAccess.Records;

namespace IpLookupProxy.Api.DataAccess;

public interface IIpInfoRepository
{
    Task<bool> IsIpCachedAsync(string ipAddress);

    Task<IpInfoRecord?> GetIpInfoAsync(string ipAddress);

    void AddIpInfo(IpInfoRecord ipInfoRecord);

    Task SaveChangesAsync();
}
