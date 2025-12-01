using SantanderTest.Models;
using System.Net.Http;

namespace SantanderTest.Clients
{
    public class HackerNewsClient : IHackerNewsClient
    {
        private readonly HttpClient _http;
        private const string BaseUrl = "https://hacker-news.firebaseio.com/v0";


        public HackerNewsClient(HttpClient httpClient)
        {
            _http = httpClient;
        }


        public async Task<int[]> GetBestStoryIdsAsync()
        {
            return await _http.GetFromJsonAsync<int[]>($"{BaseUrl}/beststories.json")
            ?? Array.Empty<int>();
        }


        public async Task<HackerNewsStory?> GetStoryAsync(int id)
        {
            return await _http.GetFromJsonAsync<HackerNewsStory>($"{BaseUrl}/item/{id}.json");
        }
    }
}
