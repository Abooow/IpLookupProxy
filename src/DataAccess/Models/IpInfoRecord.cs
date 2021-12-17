using IpLookupProxy.Api.IpResponseModels;
using MongoDB.Bson.Serialization.Attributes;

namespace IpLookupProxy.Api.DataAccess.Models;

public class IpInfoRecord : IIpInfoModel
{
    [BsonId]
    public string Ip { get; set; }

    public string City { get; set; }

    public string Region { get; set; }

    public string CountryCode { get; set; }

    public string CountryName { get; set; }

    public string Zip { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Timezone { get; set; }

    public bool? IsProxy { get; set; }

    public DateTime CachedDate { get; set; }
}
