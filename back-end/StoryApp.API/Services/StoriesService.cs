using Microsoft.Extensions.Caching.Memory;
using StoryApp.API.Helpers;
using StoryApp.API.Models;
using System.Buffers;
using System.Net.Http;

namespace StoryApp.API.Services
{
    public class StoriesService(ICacheBackService cacheBackService, ILogger<StoriesService> logger, IConfiguration configuration) : IStoriesService
    {
        private readonly string newStoriesApi = configuration["Apis:NewStories"];
        private readonly string storyApi = configuration["Apis:Story"];

        /// <inheritdoc/>
        public async Task<PagedResult<StoryModel>> GetNewStoriesByPageAsync(int pageNumber, int pageSize)
        {            
            logger.LogDebug($"Calling StoryService.GetNewStoriesByPageAsync(pageNumber:{pageNumber}, pageSize:{pageSize})");

            var storyIds = await cacheBackService.GetStoryIdsFromCacheBackAsync(CacheKeys.NEW_STORY_IDS_CACHE_KEY, newStoriesApi);

            if (storyIds == null || !storyIds.Any())
            {
                return new();
            }

            var (vPageNumber, vPageSize) = PaginationHelper.ValidatePaging(pageNumber, pageSize, storyIds.Count);

            var pagedStoryIds = PaginationHelper.GetPagedList<int>(storyIds, vPageNumber, vPageSize);

            var storyTasks = pagedStoryIds.Select(GetStoryTask);

            var stories = (await Task.WhenAll(storyTasks)).Where(s => s != null).ToList();

            return new() 
            {
                Stories = stories,
                CurrentPage = vPageNumber,
                PageSize = vPageSize,
                TotalRecords = storyIds.Count
            };
        }

        /// <inheritdoc/>
        public async Task<PagedResult<StoryModel>> SearchNewStoriesByPageAsync(int pageNumber, int pageSize, string searchValue)
        {
            logger.LogDebug($"Calling StoryService.SearchNewStoriesByPageAsync(pageNumber:{pageNumber}, pageSize:{pageSize}, searchValue: {searchValue})");

            var stories = await GetStoriesAsync();

            var filteredStories = stories.Where(s => s.Title.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            var (vPageNumber, vPageSize) = PaginationHelper.ValidatePaging(pageNumber, pageSize, filteredStories.Count);

            var pagedStories = PaginationHelper.GetPagedList(filteredStories, vPageNumber, vPageSize);

            return new()
            {
                Stories = pagedStories.ToList(),
                CurrentPage = vPageNumber,
                PageSize = vPageSize,
                TotalRecords = filteredStories.Count
            };
        }

        private Task<StoryModel> GetStoryTask(int storyId)
        {
            var key = $"{CacheKeys.STORY_CACHE_KEY}_{storyId}";

            return cacheBackService.GetStoryFromCacheBackAsync(key, $"{storyApi}/{storyId}.json");
        }

        private async Task<List<StoryModel>> GetStoriesAsync()
        { 
            List<StoryModel> stories = cacheBackService.GetStoriesFromCache(CacheKeys.STORIES_CACHE_KEY);

            if (stories == null)
            {
                stories = await GetStoriesFromApiAsync();
            }
            
            return stories;
        }

        private async Task<List<StoryModel>> GetStoriesFromApiAsync()
        {
            var storyIds = await cacheBackService.GetStoryIdsFromCacheBackAsync(CacheKeys.NEW_STORY_IDS_CACHE_KEY, newStoriesApi);

            if (storyIds == null || !storyIds.Any())
            {
                return new List<StoryModel>();
            }

            var storyTasks = storyIds.Select(GetStoryTask);

            return (await Task.WhenAll(storyTasks))
                .Where(s => s != null)
                .ToList();
        }
    }

}
