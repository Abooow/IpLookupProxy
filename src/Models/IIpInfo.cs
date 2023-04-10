namespace IpLookupProxy.Api.Models;

public interface IIpInfo
{
    public string Ip { get; }

    public string City { get; }

    public string Region { get; }

    public string CountryCode { get; }

    public string CountryName { get; }

    public string Zip { get; }

    public double? Latitude { get; }

    public double? Longitude { get; }

    public string? Timezone { get; }

    public bool? IsProxy { get; }
}
