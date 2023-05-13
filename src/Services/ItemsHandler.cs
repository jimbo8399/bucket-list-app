using bucketlist.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bucketlist.Services
{
    public class ItemsHandler : IItemsHandler
    {
        private readonly ICosmosDbService _cosmosDbService;
        private readonly IRedisClient _redisClient;
        private static RedisKey CompletedKey = new RedisKey("completed");

        public ItemsHandler(ICosmosDbService cosmosDbService, IRedisClient redisClient)
        {
            _cosmosDbService = cosmosDbService;
            _redisClient = redisClient;
        }

        public async Task<IEnumerable<Item>> GetAllCompletedItemsAsync()
        {
            var cahedCompleted = await _redisClient.GetEntryAsync(CompletedKey);
            if (cahedCompleted == null)
            {
                var completed = await _cosmosDbService.FindAllCompletedItems();
                if (completed == null)
                    return null;

                await _redisClient.AddEntryAsync(CompletedKey, completed);
                return completed;
            }

            return cahedCompleted;
        }

        public async Task MarkAsCompletedItem(Item item)
        {
            item.IsCompleted = true;
            await _cosmosDbService.UpdateItemAsync(item.Id, item);

            var cachedCompleted = await _redisClient.GetEntryAsync(CompletedKey);
            cachedCompleted = cachedCompleted.Append(item);

            await _redisClient.AddEntryAsync(CompletedKey, cachedCompleted);
        }

        public async Task CreateNewItem(Item item)
        {
            item.Id = Guid.NewGuid().ToString();
            await _cosmosDbService.AddItemAsync(item);
        }

        public async Task UpdateItem(Item item)
        {
            await _cosmosDbService.UpdateItemAsync(item.Id, item);
        }

        public async Task<Item> GetItem(string id)
        {
            return await _cosmosDbService.GetItemAsync(id);
        }
    }
}
