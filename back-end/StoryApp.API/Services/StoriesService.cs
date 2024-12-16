using Microsoft.Extensions.Caching.Memory;
using StoryApp.API.Models;
using System.Net.Http;

namespace StoryApp.API.Services
{
    public class StoriesService : IStoriesService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;
        private readonly string _newStoriesApi;
        private readonly string _storyApi;

        public StoriesService(HttpClient httpClient, IMemoryCache memoryCache, ILogger<StoriesService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _logger = logger;
            _newStoriesApi = configuration["Apis:NewStories"];
            _storyApi = configuration["Apis:Story"];
        }

        /// <inheritdoc/>
        public async Task<PagedResult<StoryModel>> GetNewStoriesByPageAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogDebug($"Calling StoryService.GetNewStoriesByPageAsync(pageNumber:{pageNumber}, pageSize:{pageSize})");
                var storyIds = await GetStoryIdsFromCacheAsync();

                if (storyIds == null || !storyIds.Any()) 
                {
                    return new();
                }

                var (vPageNumber, vPageSize) = ValidatePaging(pageNumber, pageSize, storyIds.Count());

                var pagedStoryIds = GetPagedStoryIds(storyIds, vPageNumber, vPageSize);

                var storyTasks = pagedStoryIds.Select(GetStoryFromCacheAsync);

                var stories = (await Task.WhenAll(storyTasks)).Where(s => s != null).ToList();

                
                return new() 
                {
                    Stories = stories,
                    CurrentPage = vPageNumber,
                    PageSize = vPageSize,
                    TotalRecords = storyIds.Count
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private (int PageNumber, int PageSize) ValidatePaging(int pageNumber, int pageSize, int totalCount)
        {
            int size = pageSize > 0 ? pageSize : 10;
            int totalPages = (int)Math.Ceiling((double)totalCount / size);

            int number = pageNumber > 0 ? Math.Min(pageNumber, totalPages) : 1;

            return (number, size);
        }

        private async Task<List<int>> GetStoryIdsFromCacheAsync()
        {            
            List<int> storyIds;

            if (!_memoryCache.TryGetValue(CacheKeys.NEW_STORY_IDS_CACHE_KEY, out storyIds))
            {
                _logger.LogDebug($"Fetching story Ids from API.");
                var response = await _httpClient.GetAsync(_newStoriesApi);
                response.EnsureSuccessStatusCode();
                storyIds = await response.Content.ReadFromJsonAsync<List<int>>();

                // Store in cache for 2 minutes
                _memoryCache.Set(CacheKeys.NEW_STORY_IDS_CACHE_KEY, storyIds, TimeSpan.FromMinutes(2));
            }
            else
            {
                _logger.LogDebug($"Fetching story Ids from Cache.");
            }

            return storyIds;
        }

        private async Task<StoryModel> GetStoryFromCacheAsync(int storyId)
        {
            StoryModel story;
            var key = $"{CacheKeys.STORY_CACHE_KEY}_{storyId}";

            if (!_memoryCache.TryGetValue(key, out story))
            {
                _logger.LogDebug($"Fetching story {storyId} from API.");
                var response = await _httpClient.GetAsync($"{_storyApi}/{storyId}.json");
                response.EnsureSuccessStatusCode();
                story = await response.Content.ReadFromJsonAsync<StoryModel>();

                // Store in cache for 5 minutes
                _memoryCache.Set(key, story, TimeSpan.FromMinutes(5));
            }
            else
            {
                _logger.LogDebug($"Fetching story {storyId} from Cache.");
            }

            return story;
        }

        private List<int> GetPagedStoryIds(List<int> storyIds, int page, int size)
        {
            return storyIds.Skip((page - 1) * size)
                           .Take(size)
                           .ToList();
        }
    }

}
