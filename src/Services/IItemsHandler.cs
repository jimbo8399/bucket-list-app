using bucketlist.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace bucketlist.Services
{
    public interface IItemsHandler
    {
        Task<IEnumerable<Item>> GetAllCompletedItemsAsync();
        Task MarkAsCompletedItem(Item item);
        Task CreateNewItem(Item item);
        Task UpdateItem(Item item);
        Task<Item> GetItem(string id);
    }
}
