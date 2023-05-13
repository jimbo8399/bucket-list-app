using System.Collections.Generic;

namespace bucketlist.Models
{
    public class BucketListModel
    {
        public IEnumerable<Item> TopFiveCheapestItems { get; set; }
        public IEnumerable<Item> TopFiveClosestItems { get; set; }
        public IEnumerable<Item> AllCompletedItems { get; set; }
    }
}
