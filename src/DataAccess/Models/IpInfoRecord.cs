using IpLookupProxy.Api.IpResponseModels;
using MongoDB.Bson.Serialization.Attributes;

namespace IpLookupProxy.Api.DataAccess.Models;

public class IpInfoRecord : IIpInfoModel
{
    [BsonId]
    public string Ip { get; set; } = default!;

    public string City { get; set; } = default!;

    public string Region { get; set; } = default!;

    public string CountryCode { get; set; } = default!;

    public string CountryName { get; set; } = default!;

    public string Zip { get; set; } = default!;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Timezone { get; set; }

    public bool? IsProxy { get; set; }

    public string CachedByClient { get; set; } = default!;

    public DateTime CachedDate { get; set; }
}
