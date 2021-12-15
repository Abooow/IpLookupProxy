using IpLookupProxy.Api.DataAccess.Models;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace IpLookupProxy.Api.DataAccess.Repositories;

public class MongoDbRepository : IIpRepository
{
    private const string IpInfoTableName = "IpInfos";

    private readonly IMongoDatabase _database;
    private readonly ConcurrentBag<IpInfoRecord> _cachedRecords;

    public MongoDbRepository(string connectionString, string database)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(database);

        _cachedRecords = new ConcurrentBag<IpInfoRecord>();
    }

    public async Task<bool> IsIpCachedAsync(string ipAddress)
    {
        return (await GetIpInfoAsync(ipAddress)) is not null;
    }

    public void AddIpInfo(IpInfoRecord ipInfoRecord)
    {
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
