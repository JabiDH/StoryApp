namespace StoryApp.API.Helpers
{
    public class PaginationHelper
    {
        public static (int PageNumber, int PageSize) ValidatePaging(int pageNumber, int pageSize, int totalCount)
        {
            int size = pageSize > 0 ? pageSize : 10;
            int totalPages = (int)Math.Ceiling((double)totalCount / size);

            int number = pageNumber > 0 ? Math.Min(pageNumber, totalPages) : 1;

            return (number, size);
        }


        public static IEnumerable<T> GetPagedList<T>(IEnumerable<T> list, int page, int size)
        {
            return list.Skip((page - 1) * size)
                       .Take(size)
                       .ToList();
        }
    }
}
