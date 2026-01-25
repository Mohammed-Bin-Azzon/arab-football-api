namespace api_training.Shared.Helpers
{
    public class PaginatedResult<T>
    {

        public PaginatedResult(List<T> items)
        {
            Items = items;
        }
        public List<T> Items { get; set; }

        public PaginatedResult(List<T>item=default,int count=0,int page=1,int pageSize=10 )
        {
            this.Items = item;
            CurrentPage= page;
            PageSize= pageSize;
            TotalCount= count;      
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);
        }

        public static PaginatedResult<T> Success(List<T> items, int count, int page, int pageSize)
        {
            return new(items, count, page, pageSize);
        }
        public int CurrentPage {get; set; }
        public int TotalPage { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage => CurrentPage>1;
        public bool HasnextPage => ( PageSize* CurrentPage)>TotalCount;

    }
}
