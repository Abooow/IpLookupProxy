using IpLookupProxy.Api.DataAccess.Models;
using IpLookupProxy.Api.DataAccess.Repositories;
using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.IpResponseModels;
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

    public async Task<IIpInfoModel> GetIpInfoAsync(string ipAddress)
    {
        ipAddress = ipAddress.Trim();
        if (!IPAddress.TryParse(ipAddress, out _))
            throw new InvalidIpAddressException(ipAddress);

        var cachedIpInfo = await _ipRepository.GetIpInfoAsync(ipAddress);
        if (cachedIpInfo is not null)
            return cachedIpInfo;

        var fechedIpInfo = await FetchIpInfoAsync(ipAddress);

        _ipRepository.AddIpInfo(fechedIpInfo);
        await _ipRepository.SaveChangesAsync();

        return fechedIpInfo;
    }

    private async Task<IpInfoRecord> FetchIpInfoAsync(string ipAddress)
    {
        var badClientApiKeyExceptions = new List<BadClientApiKeyException>();

        var clients = _clientRateLimiter.GetClients();
        foreach (var client in clients)
        {
            bool shouldThrottle = _clientRateLimiter.ShouldThrottleClient(client, 1, out _);
            if (shouldThrottle)
                continue;

            try
            {
                var ipClient = _ipClientsFactory.GetIpHttpClient(client);
                var responseModel = await ipClient.GetInfoAsync(ipAddress);

                return new IpInfoRecord()
                {
                    Ip = responseModel.Ip,
                    City = responseModel.City,
                    Region = responseModel.Region,
                    CountryCode = responseModel.CountryCode,
                    CountryName = responseModel.CountryName,
                    Zip = responseModel.Zip,
                    Latitude = responseModel.Latitude,
                    Longitude = responseModel.Longitude,
                    Timezone = responseModel.Timezone,
                    IsProxy = responseModel.IsProxy
                };
            }
            catch (BadClientApiKeyException e)
            {
                badClientApiKeyExceptions.Add(e);
                continue;
            }
            catch
            {
                continue;
            }
        }

        throw badClientApiKeyExceptions.Count == clients.Count()
            ? new BadClientApiKeyException(string.Join(", ", badClientApiKeyExceptions.Select(x => x.Message)))
            : new AllClientsThrottledException();
    }
}
