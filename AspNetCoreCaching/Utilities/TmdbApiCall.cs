using AspNetCoreCaching.JsonModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCoreCaching.Utilities
{
    public static class TmdbApiCall
    {
        const string url = "https://api.themoviedb.org/3/";
        const string apiKey = "your API key";

        public async static Task<List<string>> GetMovieList(string actorName)
        {
            var result = new List<string>();

            int id = await GetActorId(actorName);

            string urlParameters = $"person/{id}/movie_credits?api_key={apiKey}&language=en-US";
            var movieList = await GetAsync<MovieList>(url, urlParameters);
            if (movieList != null)
            {
                foreach (var movie in movieList.Movies)
                {
                    result.Add(movie.Title);
                }
            }
            return result;
        }

        private async static Task<int> GetActorId(string actorName)
        {
            actorName = WebUtility.UrlEncode(actorName);
            string urlParameters = $"search/person?api_key={apiKey}&query={actorName}";
            var actorList = await GetAsync<ActorList>(url, urlParameters);
            if (actorList != null && actorList.Actors.Count > 0)
            {
                return actorList.Actors[0].Id;
            }
            return -1;
        }


        private static HttpClient GetHttpClient(string url)
        {
            var client = new HttpClient { BaseAddress = new Uri(url) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        private static async Task<T> GetAsync<T>(string url, string urlParameters)
        {
            try
            {
                using (var client = GetHttpClient(url))
                {
                    HttpResponseMessage response = await client.GetAsync(urlParameters);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(json);
                        return result;
                    }

                    return default;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}

