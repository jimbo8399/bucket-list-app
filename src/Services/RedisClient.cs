namespace bucketlist.Services;

using bucketlist.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

public class RedisClient : IRedisClient
{
    private static IDatabase _redisDb;

    public RedisClient(IDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    public async Task<bool> DeleteEntry(string key)
    {
        var redisKey = new RedisKey(key);
        return await _redisDb.KeyDeleteAsync(redisKey);
    }

    public async Task<bool> AddEntry(string key, Item[] items)
    {
        var redisKey = new RedisKey(key);
        var itemsAsJson = JsonConvert.SerializeObject(items);
        var redisValue = new RedisValue(itemsAsJson);
        return await _redisDb.StringSetAsync(redisKey, redisValue, TimeSpan.FromDays(30), When.Always);
    }

    public async Task<Item[]> GetEntry(string key)
    {
        var redisKey = new RedisKey(key);
        var redisValue = await _redisDb.StringGetAsync(redisKey);

        if (!redisValue.HasValue)
            return null;

        return JsonConvert.DeserializeObject<Item[]>(redisValue);
    }
}
