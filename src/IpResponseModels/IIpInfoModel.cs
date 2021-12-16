namespace IpLookupProxy.Api.IpResponseModels;

public interface IIpInfoModel
{
    public string Ip { get; }
    public string City { get; }
    public string Region { get; }
    public string CountryCode { get; }
    public string CountryName { get; }
}
