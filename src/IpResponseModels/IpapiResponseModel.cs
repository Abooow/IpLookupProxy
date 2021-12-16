using System.Text.Json.Serialization;

namespace IpLookupProxy.Api.IpResponseModels;

public class IpapiResponseModel : IIpInfoModel
{
    public string Ip { get; set; }

    public string City { get; set; }

    [JsonPropertyName("region_name")]
    public string Region { get; set; }

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }

    [JsonPropertyName("country_name")]
    public string CountryName { get; set; }
}
