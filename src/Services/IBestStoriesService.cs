using SantanderTest.Models;

namespace SantanderTest.Services
{
    public interface IBestStoriesService
    {
        Task<IEnumerable<StoryDto>> GetBestStoriesAsync(int n);
    }
}
