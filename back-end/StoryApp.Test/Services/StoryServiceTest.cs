using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using StoryApp.API.Models;
using StoryApp.API.Services;
using StoryApp.Test.Mocks;


namespace StoryApp.Test.Services
{
    [TestClass]
    public class StoryServiceTest
    {
        private const string STORIES_API = "http://test/api/stories";
        private const string NEW_STORIES_API = $"{STORIES_API}/new";

        private MockHttpMessageHandler _httpHandlerMock;
        private IMemoryCache _memoryCache;
        private Mock<ILogger<StoriesService>> _loggerMock;
        private IConfiguration _configuration;
        private StoriesService _storyService;

        [TestInitialize]
        public void SetUp()
        {
            _httpHandlerMock = new MockHttpMessageHandler();
            _loggerMock = new Mock<ILogger<StoriesService>>();

            var cacheOptions = new MemoryCacheOptions();
            _memoryCache = new MemoryCache(cacheOptions);

            var configurations = new Dictionary<string, string>
            {
                { "Apis:NewStories", NEW_STORIES_API },
                { "Apis:Story", STORIES_API }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurations)
                .Build();

            var httpClient = new HttpClient(_httpHandlerMock);
            _storyService = new StoriesService(httpClient, _memoryCache, _loggerMock.Object, _configuration);
        }

        [TestMethod]
        public async Task GetNewStoriesByPageAsync_ShouldReturnPagedStories_DataExists()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3, 4, 5 };
            _httpHandlerMock.SetupResponse(NEW_STORIES_API, storyIds);

            SetupStoriesResponses(storyIds);

            // Act
            var result = await _storyService.GetNewStoriesByPageAsync(1, 3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Stories.Count);
            Assert.AreEqual(1, result.CurrentPage);
            Assert.AreEqual(3, result.PageSize);
            Assert.AreEqual(5, result.TotalRecords);
        }

        [TestMethod]
        public async Task GetNewStoriesByPageAsync_ShouldHandleEmptyStoryIds()
        {
            // Arrange
            _httpHandlerMock.SetupResponse(NEW_STORIES_API, new List<int>());

            // Act
            var result = await _storyService.GetNewStoriesByPageAsync(1, 3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Stories.Count);
            Assert.AreEqual(0, result.TotalRecords);
        }

        [TestMethod]
        public async Task GetNewStoriesByPageAsync_ShouldGetStoriesAccordingToPageParams()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3, 4, 5 };
            _httpHandlerMock.SetupResponse(NEW_STORIES_API, storyIds);

            SetupStoriesResponses(storyIds);

            // Act
            var result = await _storyService.GetNewStoriesByPageAsync(2, 3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Stories.Count); // Second page contains 2 stories
            Assert.AreEqual(2, result.CurrentPage);
            Assert.AreEqual(3, result.PageSize);
            Assert.AreEqual(5, result.TotalRecords);
        }

        [TestMethod]
        public async Task GetNewStoriesByPageAsync_ValidateStoriesBeingCached()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3 };
            var stories = storyIds.Select(id => new StoryModel { Id = id, Title = $"Story {id}" }).ToList();
            _httpHandlerMock.SetupResponse(NEW_STORIES_API, storyIds);
            SetupStoriesResponses(storyIds);

            // Act
            var result = await _storyService.GetNewStoriesByPageAsync(1, 3);
            List<int> cachedStoryIds;
            var isCached = _memoryCache.TryGetValue(CacheKeys.NEW_STORY_IDS_CACHE_KEY, out cachedStoryIds);
            List<StoryModel> cachedStories = new List<StoryModel>();
            foreach (var storyId in storyIds)
            {
                StoryModel cachedStory;
                _memoryCache.TryGetValue($"{CacheKeys.STORY_CACHE_KEY}_{storyId}", out cachedStory);
                cachedStories.Add(cachedStory);
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Stories.Count);
            Assert.IsTrue(isCached);
            Assert.IsNotNull(cachedStoryIds);
            Assert.AreEqual(3, cachedStoryIds.Count);
            Assert.IsNotNull(cachedStories);
            Assert.AreEqual(3, cachedStories.Count);
        }

        private void SetupStoriesResponses(List<int> storyIds)
        {
            foreach (var id in storyIds)
            {
                _httpHandlerMock.SetupResponse($"{STORIES_API}/{id}.json", new StoryModel { Id = id, Title = $"Story {id}" });
            }
        }
    }
}
