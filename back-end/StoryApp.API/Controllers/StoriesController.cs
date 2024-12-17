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
        /// <param name="page">The page number</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns>Paged list of stories</returns>
        [HttpGet("new")]
        public async Task<IActionResult> GetNewStories(int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogDebug($"Calling StoriesController.GetNewStories(pageNumber:{pageNumber}, pageSize:{pageSize})");

            var pageResult = await _storiesService.GetNewStoriesByPageAsync(pageNumber, pageSize);

            return Ok(pageResult);
        }
    }
}
