using Newtonsoft.Json;
using System.Collections.Generic;

namespace AspNetCoreCaching.JsonModels
{
    public class MovieList
    {
        [JsonProperty("cast")]
        public List<Movie> Movies { get; set; }
    }
}
