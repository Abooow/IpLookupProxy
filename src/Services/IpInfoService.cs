using System.Net;
using IpLookupProxy.Api.DataAccess.Models;
using IpLookupProxy.Api.DataAccess.Repositories;
using IpLookupProxy.Api.Exceptions;
using IpLookupProxy.Api.IpResponseModels;

namespace IpLookupProxy.Api.Services;

internal class IpInfoService
{
    private readonly IIpRepository _ipRepository;
    private readonly IIpClientLoadBalancer _ipClientLoadBalancer;
    private readonly IpClientsFactory _ipClientsFactory;

    public IpInfoService(IIpRepository ipRepository, IIpClientLoadBalancer ipClientLoadBalancer, IpClientsFactory ipClientsFactory)
    {
        _ipRepository = ipRepository;
        _ipClientLoadBalancer = ipClientLoadBalancer;
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
        if (fechedIpInfo.Exists)
        {
            _ipRepository.AddIpInfo(fechedIpInfo);
            await _ipRepository.SaveChangesAsync();
        }

        return fechedIpInfo;
    }

    private async Task<IpInfoRecord> FetchIpInfoAsync(string ipAddress)
    {
        if (IsInternalIpAddress(ipAddress))
            return new IpInfoRecord() { Ip = ipAddress };

        string clientName = _ipClientLoadBalancer.GetClient();
        var ipClient = _ipClientsFactory.GetIpHttpClient(clientName);

        try
        {
            var responseModel = await ipClient.GetInfoAsync(ipAddress);

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
