using SantanderTest.Models;

namespace SantanderTest.Clients
{
    public interface IHackerNewsClient
    {
        Task<int[]> GetBestStoryIdsAsync();
        Task<HackerNewsStory?> GetStoryAsync(int id);
    }
}
