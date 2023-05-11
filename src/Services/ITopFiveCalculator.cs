using System.Collections.Generic;
using System.Threading.Tasks;
using bucketlist.Models;

namespace bucketlist.Services;

public interface ITopFiveCalculator
{
    Task<IEnumerable<Item>> GetTopFiveCheapest();
    Task<IEnumerable<Item>> GetTopFiveClosest();
    Task<IEnumerable<Item>> GetTopFiveOldest();
}
