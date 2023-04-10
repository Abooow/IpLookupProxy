using IpLookupProxy.Api.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace IpLookupProxy.Api.DataAccess.Records;

public class IpInfoRecord : IIpInfo
{
    [BsonId]
    public string Ip { get; set; } = default!;

    public bool Exists { get; set; }

    public string City { get; set; } = default!;

    public string Region { get; set; } = default!;

    public string CountryCode { get; set; } = default!;

    public string CountryName { get; set; } = default!;

    public string Zip { get; set; } = default!;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Timezone { get; set; }

    public bool? IsProxy { get; set; }

    public string FetchedFromClient { get; set; } = default!;

    public DateTime CachedDate { get; set; }
}
