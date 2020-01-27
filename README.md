# AspNetCoreCaching
This project demonstrates how to use in-memory and Redis-based distributed caching in an ASP.NET Core Web API.

To run the project you need to get an API key from [here](https://developers.themoviedb.org/3/getting-started/introduction) as the API makes an 
external API call to TMDB site.

Then update the following line in `TmdbApiCall` class with your API key:

```
const string apiKey = "your API key";
```

This project was originally developed as a demo project for the following post:

[In-memory & Distributed (Redis) Caching in ASP.NET Core](https://medium.com/net-core/in-memory-distributed-redis-caching-in-asp-net-core-62fb33925818)
