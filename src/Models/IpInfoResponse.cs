namespace IpLookupProxy.Api.Models;

public sealed record IpInfoResponse(
    string Ip,
    string City,
    string Region,
    string CountryCode,
    string CountryName,
    string Zip,
    double? Latitude,
    double? Longitude,
    string? Timezone,
    bool? IsProxy);