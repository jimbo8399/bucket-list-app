namespace bucketlist.Services;

using bucketlist.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

public class RedisClient : IRedisClient
{
    private static IDatabase _redisDb;
    private const string PickedKeyName = "picked";

    public RedisClient(IDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    public async Task<bool> DeletePickedEntryAsync()
    {
        var redisKey = new RedisKey(PickedKeyName);
        return await _redisDb.KeyDeleteAsync(redisKey);
    }

    public async Task<bool> UpdatePickedEntryAsync(Item item)
    {
        var redisKey = new RedisKey(PickedKeyName);
        var itemAsJson = JsonConvert.SerializeObject(item);
        var redisValue = new RedisValue(itemAsJson);
        return await _redisDb.StringSetAsync(redisKey, redisValue, TimeSpan.MaxValue, When.Always);
    }

    public async Task<Item> GetPickedEntryAsync()
    {
        var redisKey = new RedisKey(PickedKeyName);
        var redisValue = await _redisDb.StringGetAsync(redisKey);

        if (!redisValue.HasValue)
            return null;

        var item = JsonConvert.DeserializeObject<Item>(redisValue);
        return item;
    }

    public async Task<bool> DeleteEntryAsync(string key)
    {
        var redisKey = new RedisKey(key);
        return await _redisDb.KeyDeleteAsync(redisKey);
    }

    public async Task<bool> AddEntryAsync(string key, Item[] items)
    {
        var redisKey = new RedisKey(key);
        var itemsAsJson = JsonConvert.SerializeObject(items);
        var redisValue = new RedisValue(itemsAsJson);
        return await _redisDb.StringSetAsync(redisKey, redisValue, TimeSpan.FromDays(30), When.Always);
    }

    public async Task<Item[]> GetEntryAsync(string key)
    {
        var redisKey = new RedisKey(key);
        var redisValue = await _redisDb.StringGetAsync(redisKey);

        if (!redisValue.HasValue)
            return null;

        return JsonConvert.DeserializeObject<Item[]>(redisValue);
    }
}
