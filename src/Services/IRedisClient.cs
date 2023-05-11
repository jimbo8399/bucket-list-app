using bucketlist.Models;
using System.Threading.Tasks;

namespace bucketlist.Services;

public interface IRedisClient
{
    Task<bool> DeleteEntry(string key);

    Task<bool> AddEntry(string key, Item[] items);

    Task<Item[]> GetEntry(string key);
}
