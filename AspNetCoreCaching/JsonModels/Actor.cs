using Newtonsoft.Json;

namespace AspNetCoreCaching.JsonModels
{
    public class Actor
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}
