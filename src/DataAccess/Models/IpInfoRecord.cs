using MongoDB.Bson.Serialization.Attributes;

namespace IpLookupProxy.Api.DataAccess.Models;

public class IpInfoRecord
{
    [BsonId]
    public string Ip { get; set; }

    public string City { get; set; }

    public string Region { get; set; }

    public string CountryCode { get; set; }

    public string CountryName { get; set; }
}
