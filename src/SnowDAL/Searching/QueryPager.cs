namespace SnowDAL.Searching
{
    public class QueryPager : IQueryPaging
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
