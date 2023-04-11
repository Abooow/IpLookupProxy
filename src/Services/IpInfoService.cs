using System.Net;
using IpLookupProxy.Api.DataAccess;
using IpLookupProxy.Api.DataAccess.Records;
using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.Models;

namespace IpLookupProxy.Api.Services;

internal class IpInfoService
{
    private readonly IIpInfoRepository _ipRepository;
    private readonly IIpClientLoadBalancer _ipClientLoadBalancer;
    private readonly IpLookupClientFactory _ipClientsFactory;

    public IpInfoService(IIpInfoRepository ipRepository, IIpClientLoadBalancer ipClientLoadBalancer, IpLookupClientFactory ipClientsFactory)
    {
        _ipRepository = ipRepository;
        _ipClientLoadBalancer = ipClientLoadBalancer;
        _ipClientsFactory = ipClientsFactory;
    }

    public async Task<IIpInfo> GetIpInfoAsync(string ipAddress)
    {
        ipAddress = ipAddress.Trim();
        if (!IPAddress.TryParse(ipAddress, out _))
            throw new InvalidIpAddressException(ipAddress);

        var cachedIpInfo = await _ipRepository.GetIpInfoAsync(ipAddress);
        if (cachedIpInfo is not null)
            return cachedIpInfo;

        var fetchedIpInfo = await FetchIpInfoAsync(ipAddress);
        if (fetchedIpInfo.Exists)
        {
            _ipRepository.AddIpInfo(fetchedIpInfo);
            await _ipRepository.SaveChangesAsync();
        }

        return fetchedIpInfo;
    }

    private async Task<IpInfoRecord> FetchIpInfoAsync(string ipAddress)
    {
        if (IsInternalIpAddress(ipAddress))
            return new IpInfoRecord() { Ip = ipAddress };

        string clientName = _ipClientLoadBalancer.GetClient();
        var ipLookupClient = _ipClientsFactory.GetIpHttpClient(clientName);

        try
        {
            var responseModel = await ipLookupClient.GetIpInfoAsync(ipAddress);

            return new IpInfoRecord()
            {
                Ip = ipAddress,
                Exists = true,
                City = responseModel.City,
                Region = responseModel.Region,
                CountryCode = responseModel.CountryCode,
                CountryName = responseModel.CountryName,
                Zip = responseModel.Zip,
                Latitude = responseModel.Latitude,
                Longitude = responseModel.Longitude,
                Timezone = responseModel.Timezone,
                IsProxy = responseModel.IsProxy,
                FetchedFromClient = clientName
            };
        }
        catch (IpDoesNotExistException)
        {
            return new IpInfoRecord()
            {
                Ip = ipAddress,
                Exists = false,
                FetchedFromClient = clientName
            };
        }
    }

    private static bool IsInternalIpAddress(string ipAddress)
    {
        int[] octets = ipAddress.Split('.').Take(2).Select(int.Parse).ToArray();
        return octets[0] switch
        {
            0 or 10 or 127 or >= 224 => true,
            172 => octets[1] is >= 16 and <= 31,
            169 => octets[1] is 254,
            192 => octets[1] is 168,
            _ => false
        };
    }
}
