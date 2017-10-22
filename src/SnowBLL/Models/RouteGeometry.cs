using SnowBLL.Models.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models
{
    public class RouteGeometry
    {
        public IEnumerable<GeoPoint> Points { get; set; }
        public int Id { get;  set; }
    }
}
