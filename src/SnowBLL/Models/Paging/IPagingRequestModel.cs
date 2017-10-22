namespace SnowBLL.Models.Routes
{
    public interface IPagingRequestModel
    {
        int? Page { get; set; }
        int? PageSize { get; set; }
    }
}