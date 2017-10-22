namespace SnowBLL.Models.Routes
{
    public class PagingRequestModel : IPagingRequestModel
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public PagingRequestModel()
        {
        }

        public PagingRequestModel(int? page, int? pagesize)
        {
            this.Page = page;
            this.PageSize = pagesize;
        }
    }
}