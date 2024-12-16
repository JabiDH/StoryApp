using StoryApp.API.Models;

namespace StoryApp.API.Services
{
    public interface IStoriesService
    {
        /// <summary>
        /// Get the newest stories with pagination
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>Paged list of stories</returns>
        Task<PagedResult<StoryModel>> GetNewStoriesByPageAsync(int pageNumber, int pageSize);
    }
}
