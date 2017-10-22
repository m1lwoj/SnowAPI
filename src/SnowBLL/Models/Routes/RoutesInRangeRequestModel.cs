using SnowBLL.Models.Geo;

namespace SnowBLL.Models.Routes
{
    public class RoutesInRangeRequestModel : PagingRequestModel
    {
        public RoutesInRangeRequestModel(int kilometers, decimal? lat, decimal? lng, int? page, int? pagesize)
        {
            Kilometers = kilometers;
            Point = new GeoPoint()
            {
                Lat = lat,
                Lng = lng
            };
            Page = page;
            PageSize = pagesize;
        }

        public GeoPoint Point { get; }

        public int Kilometers { get; }
    }
}
