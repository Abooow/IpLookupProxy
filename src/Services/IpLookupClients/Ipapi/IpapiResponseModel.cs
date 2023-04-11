using System.Text.Json.Serialization;
using IpLookupProxy.Api.Models;

namespace IpLookupProxy.Api.Services.IpLookupClients.Ipapi;

public class IpapiResponseModel : IIpInfo
{
    public string Ip { get; set; } = default!;

    public string City { get; set; } = default!;

    [JsonPropertyName("region_name")]
    public string Region { get; set; } = default!;

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; } = default!;

    [JsonPropertyName("country_name")]
    public string CountryName { get; set; } = default!;

    public string Zip { get; set; } = default!;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Timezone => IpapiTimeZone?.Id;

    public bool? IsProxy => IpapiSecurity?.IsProxy;

    [JsonPropertyName("time_zone")]
    public IpapiTimeZoneModel? IpapiTimeZone { get; set; }

    [JsonPropertyName("security")]
    public IpapiSecurityModel? IpapiSecurity { get; set; }

    public class IpapiTimeZoneModel
    {
        public string Id { get; set; } = default!;
    }

    public class IpapiSecurityModel
    {
        [JsonPropertyName("is_proxy")]
        public bool? IsProxy { get; set; }
    }
}