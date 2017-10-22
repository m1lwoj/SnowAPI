using SnowBLL.Models.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models.Routes
{
    public class RoutePoint
    {
        public GeoPoint Point { get; set; }

        public int Id { get; set; }
    }
}
