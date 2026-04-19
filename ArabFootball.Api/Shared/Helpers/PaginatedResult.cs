namespace ArabFootball.Shared.Helpers
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(List<T>? items = null, int count = 0, int page = 1, int pageSize = 10)
        {
            Items = items ?? new List<T>();
            CurrentPage = page;
            PageSize = pageSize;
            TotalCount = count;
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);
        }

        public List<T> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => (PageSize * CurrentPage) < TotalCount;

        public static PaginatedResult<T> Success(List<T> items, int count, int page, int pageSize)
        {
            return new(items, count, page, pageSize);
        }
    }
}