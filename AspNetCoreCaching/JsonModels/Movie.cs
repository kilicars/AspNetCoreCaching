using Newtonsoft.Json;

namespace AspNetCoreCaching.JsonModels
{
    public class Movie
    {
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
