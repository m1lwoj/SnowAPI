using SnowBLL.Models.Geo;
using Xunit;
using System.Linq;

namespace SnowTests.Unit
{
    public class GeoPointCoverterTests
    {
        [Fact]
        public void ConvertLinetoPoints()
        { 
            string line = "LINESTRING(431.33393335342407 3.82005181729661,47.33685159683228 32.80960466650892)";
            var points = GeoPointConverter.GetPoints(line);

            Assert.Equal(points.Count(), 2);
            Assert.Equal(points.ToArray()[0].Lat.Value, 431.33M, 2);
            Assert.Equal(points.ToArray()[0].Lng.Value, 3.82M, 2);

            string line2 = "LINESTRING(49.373544 20.078626 , 49.379992   20.087141   ,    49.384800 20.084377,49.389893 20.097433)";
            var points2 = GeoPointConverter.GetPoints(line2);

            Assert.Equal(points2.Count(), 4);
            Assert.Equal(points2.ToArray()[0].Lat.Value, 49.37M, 2);
            Assert.Equal(points2.ToArray()[0].Lng.Value, 20.08M, 2);

        }

        [Fact]
        public void ConvertPoint()
        {
            string pointString = "POINT(431.33393335342407 3.82005181729661)";
            var point = GeoPointConverter.GetPoint(pointString);

            Assert.Equal(point.Lat.Value, 431.33M, 2);
            Assert.Equal(point.Lng.Value, 3.82M, 2);
        }
    }
}
