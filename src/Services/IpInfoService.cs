using IpLookupProxy.Api.DataAccess.Models;
using IpLookupProxy.Api.DataAccess.Repositories;
using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Models;
using System.Net;

namespace IpLookupProxy.Api.Services;

internal class IpInfoService
{
    private readonly IIpRepository _ipRepository;
    private readonly ClientRateLimiter _clientRateLimiter;
    private readonly IpClientsFactory _ipClientsFactory;

    public IpInfoService(IIpRepository ipRepository, ClientRateLimiter clientRateLimiter, IpClientsFactory ipClientsFactory)
    {
        _ipRepository = ipRepository;
        _clientRateLimiter = clientRateLimiter;
        _ipClientsFactory = ipClientsFactory;
    }

    public async Task<IpInfoResult> GetIpInfoAsync(string ipAddress)
    {
        ipAddress = ipAddress.Trim();
        if (!IPAddress.TryParse(ipAddress, out _))
            throw new InvalidIpAddressException(ipAddress);

        var cachedIpInfo = await _ipRepository.GetIpInfoAsync(ipAddress);
        if (cachedIpInfo is not null)
            return IpInfoResult.Success(cachedIpInfo);

        var fechedIpInfoResult = await FetchIpInfoAsync(ipAddress);
        if (fechedIpInfoResult.IpInfo is null)
            return IpInfoResult.Fail(fechedIpInfoResult.WaitTime);

        _ipRepository.AddIpInfo(fechedIpInfoResult.IpInfo);
        await _ipRepository.SaveChangesAsync();

        return IpInfoResult.Success(fechedIpInfoResult.IpInfo);
    }

    private async Task<(IpInfoRecord? IpInfo, TimeSpan WaitTime)> FetchIpInfoAsync(string ipAddress)
    {
        var clients = _clientRateLimiter.GetClients();
        foreach (var client in clients)
        {
            bool shouldThrottle = _clientRateLimiter.ShouldThrottleClient(client, 1, out TimeSpan clientWaitTime);
            if (shouldThrottle)
                continue;

            try
            {
                var ipClient = _ipClientsFactory.GetIpHttpClient(client);
                var responseModel = await ipClient.GetInfoAsync(ipAddress);

                return (new IpInfoRecord()
                {
                    Ip = responseModel.Ip,
                    City = responseModel.City,
                    Region = responseModel.Region,
                    CountryCode = responseModel.CountryCode,
                    CountryName = responseModel.CountryName
                }, TimeSpan.Zero);
            }
            catch
            {
                continue;
            }
        }

        throw new AllClientsThrottledException();
    }
}
