using System.Collections.Concurrent;
using IpLookupProxy.Api.DataAccess.Records;
using MongoDB.Driver;

namespace IpLookupProxy.Api.DataAccess;

public class MongoDbIpInfoRepository : IIpInfoRepository
{
    private const string IpInfoTableName = "IpInfo";

    private readonly IMongoDatabase _database;
    private readonly ConcurrentBag<IpInfoRecord> _cachedRecords;

    public MongoDbIpInfoRepository(string connectionString, string database)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(database);

        _cachedRecords = new ConcurrentBag<IpInfoRecord>();
    }

    public async Task<bool> IsIpCachedAsync(string ipAddress)
    {
        return await GetIpInfoAsync(ipAddress) is not null;
    }

    public void AddIpInfo(IpInfoRecord ipInfoRecord)
    {
        ipInfoRecord.CachedDate = DateTime.UtcNow;
        _cachedRecords.Add(ipInfoRecord);
    }

    public async Task<IpInfoRecord?> GetIpInfoAsync(string ipAddress)
    {
        var collection = _database.GetCollection<IpInfoRecord>(IpInfoTableName);
        var query = await collection.FindAsync(Builders<IpInfoRecord>.Filter.Eq(x => x.Ip, ipAddress));

        return await query.FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        try
        {
            var collection = _database.GetCollection<IpInfoRecord>(IpInfoTableName);
            await collection.InsertManyAsync(_cachedRecords);
        }
        finally
        {
            _cachedRecords.Clear();
        }
    }
}
