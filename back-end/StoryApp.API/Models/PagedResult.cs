using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StoryApp.API.Models
{
    public class PagedResult<T>
    {
        public List<T> Stories { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}
