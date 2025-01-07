using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using StoryApp.API.Models;
using StoryApp.API.Services;
using System.Net.Http.Headers;

namespace StoryApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IStoriesService _storiesService;
        private readonly ILogger<StoriesController> _logger;

        public StoriesController(IStoriesService storiesService, ILogger<StoriesController> logger)
        {
            _storiesService = storiesService;
            _logger = logger;
        }

        /// <summary>
        /// Get the newest stories with pagination
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns>Paged list of stories</returns>
        [HttpGet("new")]
        public async Task<IActionResult> GetNewStories(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogDebug($"Calling StoriesController.GetNewStories(pageNumber:{pageNumber}, pageSize:{pageSize})");

            var pagedResult = await GetNewStoriesByPageAsync(pageNumber, pageSize);

            return Ok(pagedResult);
        }

        /// <summary>
        /// Lookup stories that match the search value
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <param name="searchValue">The search value</param>
        /// <returns>A list of stories</returns>
        [HttpGet("new/search")]
        public async Task<IActionResult> SearchNewStories(string? searchValue, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogDebug($"Calling StoriesController.SearchStories(searchValue: {searchValue}, pageNumber:{pageNumber}, pageSize:{pageSize})");

            if (string.IsNullOrEmpty(searchValue))
            {
                return Ok(GetNewStoriesByPageAsync(pageNumber, pageSize));
            }

            var pagedResult = await _storiesService.SearchNewStoriesByPageAsync(pageNumber, pageSize, searchValue.Trim());

            return Ok(pagedResult);
        }

        private async Task<PagedResult<StoryModel>> GetNewStoriesByPageAsync(int pageNumber, int pageSize)
        {
            return await _storiesService.GetNewStoriesByPageAsync(pageNumber, pageSize);
        }
    }
}
