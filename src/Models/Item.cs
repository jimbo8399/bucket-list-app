using Newtonsoft.Json;

namespace bucketlist.Models;

public class Item
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "price")]
    public int Price { get; set; }

    [JsonProperty(PropertyName = "distance")]
    public int Distance { get; set; }
}
