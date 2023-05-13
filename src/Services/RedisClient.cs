using bucketlist.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bucketlist.Services;

public class RedisClient : IRedisClient
{
    private static IDatabase _redisDb;

    public RedisClient(IDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    public async Task<bool> DeleteEntryAsync(string key)
    {
        var redisKey = new RedisKey(key);
        return await _redisDb.KeyDeleteAsync(redisKey);
    }

    public async Task<bool> AddEntryAsync(string key, IEnumerable<Item> items)
    {
        var redisKey = new RedisKey(key);
        var itemsAsJson = JsonConvert.SerializeObject(items);
        var redisValue = new RedisValue(itemsAsJson);
        return await _redisDb.StringSetAsync(redisKey, redisValue, TimeSpan.FromDays(15), When.Always);
    }

    public async Task<IEnumerable<Item>> GetEntryAsync(RedisKey redisKey)
    {
        var redisValue = await _redisDb.StringGetAsync(redisKey);

        if (!redisValue.HasValue)
            return null;

        return JsonConvert.DeserializeObject<IEnumerable<Item>>(redisValue);
    }
}
