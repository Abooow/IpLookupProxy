using IpLookupProxy.Api.DataAccess.Models;

namespace IpLookupProxy.Api.DataAccess.Repositories;

public interface IIpRepository
{
    Task<bool> IsIpCachedAsync(string ipAddress);

    Task<IpInfoRecord?> GetIpInfoAsync(string ipAddress);

    void AddIpInfo(IpInfoRecord ipInfoRecord);

    Task SaveChangesAsync();
}
