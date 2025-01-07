using StoryApp.API.Models;

namespace StoryApp.API.Services
{
    public interface ICacheBackService
    {      
        List<StoryModel> GetStoriesFromCache(string cacheKey);
        Task<List<int>> GetStoryIdsFromCacheBackAsync(string cacheKey, string url);
        Task<StoryModel> GetStoryFromCacheBackAsync(string cacheKey, string url);
    }
}
