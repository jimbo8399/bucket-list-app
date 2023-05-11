using bucketlist.Models;
using System.Threading.Tasks;

namespace bucketlist.Services;

public interface IRedisClient
{
    Task<bool> DeletePickedEntryAsync();

    Task<bool> UpdatePickedEntryAsync(Item item);

    Task<Item> GetPickedEntryAsync();

    Task<bool> DeleteEntryAsync(string key);

    Task<bool> AddEntryAsync(string key, Item[] items);

    Task<Item[]> GetEntryAsync(string key);
}
