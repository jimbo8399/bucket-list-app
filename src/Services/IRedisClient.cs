using bucketlist.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bucketlist.Services;

public interface IRedisClient
{
    Task<bool> DeleteEntryAsync(string key);

    Task<bool> AddEntryAsync(string key, IEnumerable<Item> items);

    Task<IEnumerable<Item>> GetEntryAsync(RedisKey redisKey);
}
