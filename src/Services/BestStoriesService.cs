using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SantanderTest.Clients;
using SantanderTest.Models;
using SantanderTest.Utils;
using System.Collections.Concurrent;
using System.Text.Json;

namespace SantanderTest.Services
{
    public class BestStoriesService : IBestStoriesService
    {
        private readonly IHackerNewsClient _client;
        private readonly IDistributedCache _cache;
        private readonly ILogger<BestStoriesService> _logger;
        private readonly GeneralSettings _settings;


        public BestStoriesService(IHackerNewsClient client, IDistributedCache cache, ILogger<BestStoriesService> logger, IOptions<GeneralSettings> settings)
        {
            _client = client;
            _cache = cache;
            _logger = logger;
            _settings=settings.Value;
        }


        public async Task<IEnumerable<StoryDto>> GetBestStoriesAsync(int n)
        {
            var cacheKey = $"beststories:{n}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null) return JsonSerializer.Deserialize<IEnumerable<StoryDto>>(cached)!;


            var ids = await _client.GetBestStoryIdsAsync();
            var take = ids.Take(n).ToArray();


            var results = new ConcurrentBag<HackerNewsStory>();
            var semaphore = new SemaphoreSlim(10); 


            await Parallel.ForEachAsync(take, new ParallelOptions { MaxDegreeOfParallelism = 20 }, async (id, ct) =>
            {
                await semaphore.WaitAsync(ct);
                try
                {
                    var item = await _client.GetStoryAsync(id);
                    if (item != null) 
                        results.Add(item);
                }
                finally { semaphore.Release(); }
            });


            var dto = results
            .OrderByDescending(s => s.Score)
            .Select(s => new StoryDto { Title = s.Title, Uri = s.Url, PostedBy = s.By, Time = DateTimeOffset.FromUnixTimeSeconds(s.Time).UtcDateTime, Score = s.Score, CommentCount = s.Descendants })
            .ToList();


            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_settings.RedisCacheSeconds) });


            return dto;
        }
    }
}
