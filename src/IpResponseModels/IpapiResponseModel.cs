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

    public string Zip { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Timezone => IpapiTimeZone?.Id;

    public bool? IsProxy => IpapiSecurity?.IsProxy;

    [JsonPropertyName("time_zone")]
    public IpapiTimeZoneModel? IpapiTimeZone { get; set; }

    [JsonPropertyName("security")]
    public IpapiSecurityModel? IpapiSecurity { get; set; }

    public class IpapiTimeZoneModel
    {
        public string Id { get; set; }
    }

    public class IpapiSecurityModel
    {
        [JsonPropertyName("is_proxy")]
        public bool? IsProxy { get; set; }
    }
}