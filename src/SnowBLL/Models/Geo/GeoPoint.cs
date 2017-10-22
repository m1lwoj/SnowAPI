using System.Globalization;
using System.Linq;

namespace SnowBLL.Models.Geo
{
    public class GeoPoint
    {
        public GeoPoint() { }

        public GeoPoint(string pointsWithSeparator)
        {
            var values = pointsWithSeparator.TrimStart(' ').TrimEnd(' ').Split(' ').Where(ps => !string.IsNullOrEmpty(ps)).ToArray();
            Lat = decimal.Parse(values[0], CultureInfo.InvariantCulture);
            Lng = decimal.Parse(values[1], CultureInfo.InvariantCulture);
        }

        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
    }
}