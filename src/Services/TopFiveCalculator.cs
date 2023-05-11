using System.Collections.Generic;
using System.Threading.Tasks;
using bucketlist.Models;

namespace bucketlist.Services;

public class TopFiveCalculator : ITopFiveCalculator
{
    public Task<IEnumerable<Item>> GetTopFiveCheapest()
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<Item>> GetTopFiveClosest()
    {
        throw new System.NotImplementedException();
    }

    public Task<IEnumerable<Item>> GetTopFiveOldest()
    {
        throw new System.NotImplementedException();
    }
}
