using Microsoft.Extensions.Caching.Memory;
using StoryApp.API.Models;
using System.Net.Http;
using System.Text.Json;

namespace StoryApp.API.Services
{
    public class CacheBackService : ICacheBackService
    {
        private const int MIN_FOR_OBJ = 5;
        private const int MIN_FOR_OBJ_LIST = 2;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;

        public CacheBackService(HttpClient httpClient, IMemoryCache memoryCache, ILogger<CacheBackService> logger)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _logger = logger;
        }
      
        public async Task<List<int>> GetStoryIdsFromCacheBackAsync(string cacheKey, string url)
        {
            List<int> storyIds;

            if (!_memoryCache.TryGetValue(cacheKey, out storyIds))
            {
                _logger.LogDebug($"Fetching story Ids from API with url {url}.");

                try
                {
                    var response = await _httpClient.GetAsync(url).ConfigureAwait(true);
                    response.EnsureSuccessStatusCode();
                    storyIds = await response.Content.ReadFromJsonAsync<List<int>>();
                    _memoryCache.Set(cacheKey, storyIds, TimeSpan.FromMinutes(MIN_FOR_OBJ_LIST));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred inside {nameof(GetStoryIdsFromCacheBackAsync)}");
                    throw;
                }
            }
            else
            {
                _logger.LogDebug($"Fetching story Ids with key {cacheKey} from Cache.");
            }

            return storyIds;

        }

        public async Task<StoryModel> GetStoryFromCacheBackAsync(string cacheKey, string url)
        {
            StoryModel story;

            if (!_memoryCache.TryGetValue(cacheKey, out story))
            {
                _logger.LogDebug($"Fetching story from API with url {url}.");

                try
                {
                    var response = await _httpClient.GetAsync(url).ConfigureAwait(true);
                    response.EnsureSuccessStatusCode();
                    story = await response.Content.ReadFromJsonAsync<StoryModel>();
                    _memoryCache.Set(cacheKey, story, TimeSpan.FromMinutes(MIN_FOR_OBJ));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred inside {nameof(GetStoryFromCacheBackAsync)}");
                    throw;
                }
            }
            else
            {
                _logger.LogDebug($"Fetching story with key {cacheKey} from Cache.");
            }

            return story;
        }

        public List<StoryModel> GetStoriesFromCache(string cacheKey)
        {
            List<StoryModel> stories;

            if (!_memoryCache.TryGetValue(cacheKey, out stories))
            {
                _logger.LogDebug($"Saving stories with key {cacheKey} into Cache.");
                _memoryCache.Set(cacheKey, stories, TimeSpan.FromMinutes(MIN_FOR_OBJ_LIST));
            }
            else
            {
                _logger.LogDebug($"Fetching stories with key {cacheKey} from Cache.");
            }

            return stories;
        }
    }
}
