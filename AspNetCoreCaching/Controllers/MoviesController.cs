using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreCaching.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace AspNetCoreCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private readonly IDistributedCache distributedCache;
        public MoviesController(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        [HttpGet]
        public string Get()
        {
            return "Please enter the name of the actor/actress at the end of the URL";
        }


        [HttpGet("{actorName}")]
        public async Task<List<string>> Get(string actorName)
        {
            return await GetMovieList(actorName);
        }

        private async Task<List<string>> GetMovieList(string actorName)
        {
            var cacheKey = actorName.ToLower();

            List<string> moviesList;
            string serializedMovies;

            var encodedMovies = await distributedCache.GetAsync(cacheKey);

            if (encodedMovies != null)
            {
                serializedMovies = Encoding.UTF8.GetString(encodedMovies);
                moviesList = JsonConvert.DeserializeObject<List<string>>(serializedMovies);
            }
            else
            {
                moviesList = await TmdbApiCall.GetMovieList(actorName);
                serializedMovies = JsonConvert.SerializeObject(moviesList);
                encodedMovies = Encoding.UTF8.GetBytes(serializedMovies);
                var options = new DistributedCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                                .SetAbsoluteExpiration(DateTime.Now.AddHours(6));
                await distributedCache.SetAsync(cacheKey, encodedMovies, options);
            }
            return moviesList;
        }
    }


    #region Controller using In-Memory cache 
    /*
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private readonly IMemoryCache memoryCache;
        public MoviesController(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        public string Get()
        {
            return "Please enter the name of the actor/actress at the end of the URL";
        }

        [HttpGet("{actorName}")]
        public async Task<List<string>> Get(string actorName)
        {
            return await GetMovieList(actorName);
        }

        private async Task<List<string>> GetMovieList(string actorName)
        {
            var cacheKey = actorName.ToLower();

            if (!memoryCache.TryGetValue(cacheKey, out List<string> movieList))
            {
                movieList = await TmdbApiCall.GetMovieList(actorName);

                var cacheExpirationOptions =
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTime.Now.AddHours(6),
                        Priority = CacheItemPriority.Normal,
                        SlidingExpiration = TimeSpan.FromMinutes(5) 
                    };
                memoryCache.Set(cacheKey, movieList, cacheExpirationOptions);
            }
            return movieList;
        }

        [HttpGet("[action]")]
        public async Task<List<string>> Test()
        {
            string key = "Brad Pitt";
            memoryCache.Remove(key);
            int count = 10;

            var list = new List<string>();

            list.Add($"Total time taken to get the movie list of {key} {count} times");

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < count; i++)
            {
                await GetMovieList(key);
            }
            watch.Stop();
            double time = watch.Elapsed.TotalSeconds;
            list.Add($"In-memory caching: {time.ToString()} seconds");

            watch.Restart();
            for (int i = 0; i < count; i++)
            {
                await TmdbApiCall.GetMovieList(key);
            }
            watch.Stop();
            time = watch.Elapsed.TotalSeconds;
            list.Add($"No cache: {time.ToString()} seconds");

            return list;
        }

    }
    */
    #endregion
}