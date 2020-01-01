namespace Outmatch.API.Helpers
{
    public class UserParams
    {
        // Pagination setup to determine how many pages are needed in total, and how many items per page.
        private const int MaxPageSize = 75;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value ;}
        }
        
    }
}