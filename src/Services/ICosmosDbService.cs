using System.Collections.Generic;
using System.Threading.Tasks;
using bucketlist.Models;

namespace bucketlist.Services;

public interface ICosmosDbService
{
    Task<IEnumerable<Item>> GetItemsAsync(string query);
    Task<Item> GetItemAsync(string id);
    Task AddItemAsync(Item item);
    Task<IEnumerable<Item>> FindAllNonCompletedItems();
    Task<IEnumerable<Item>> FindAllCompletedItems();
    Task UpdateItemAsync(string id, Item item);
    Task DeleteItemAsync(string id);
}
