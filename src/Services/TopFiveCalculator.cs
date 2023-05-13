using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bucketlist.Models;
using StackExchange.Redis;

namespace bucketlist.Services;

public class TopFiveCalculator : ITopFiveCalculator
{
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IRedisClient _redisClient;
    private static RedisKey _topFiveCheapestKey = new RedisKey("topfivecheapest");
    private static RedisKey _topFiveClosestKey = new RedisKey("topfiveclosest");

    public TopFiveCalculator(ICosmosDbService cosmosDbService, IRedisClient redisClient)
    {
        _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
        _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
    }

    public async Task<IEnumerable<Item>> GetTopFiveCheapest()
    {
        var cheapest = await _redisClient.GetEntryAsync(_topFiveCheapestKey);

        if (cheapest == null)
        {
            return await calculateAndStoreTopFiveCheapest();
        }

        return cheapest;
    }


    public async Task<IEnumerable<Item>> GetTopFiveClosest()
    {
        var closest = await _redisClient.GetEntryAsync(_topFiveClosestKey);

        if (closest == null)
        {
            return await calculateAndStoreTopFiveClosest();
        }

        return closest;
    }

    public async Task CalculateAndStoreTopFives()
    {
        var items = await _cosmosDbService.FindAllNonCompletedItems();

        await Task.WhenAll(new[] {
                calculateAndStoreTopFiveCheapest(items),
                calculateAndStoreTopFiveClosest(items)
            });
    }

    private async Task<IEnumerable<Item>> calculateAndStoreTopFiveCheapest(IEnumerable<Item> items = null)
    {
        if (items == null)
        {
            items = await _cosmosDbService.FindAllNonCompletedItems();
        }

        var orderedItems = items.OrderBy(item => item.Price);
        var topFive = orderedItems.Take(5);

        await _redisClient.AddEntryAsync(_topFiveCheapestKey, topFive);

        return topFive;
    }

    private async Task<IEnumerable<Item>> calculateAndStoreTopFiveClosest(IEnumerable<Item> items = null)
    {
        if (items == null)
        {
            items = await _cosmosDbService.FindAllNonCompletedItems();
        }
        
        var orderedItems = items.OrderBy(item => item.Distance);
        var topFive = orderedItems.Take(5);

        await _redisClient.AddEntryAsync(_topFiveClosestKey, topFive);

        return topFive;
    }
}
