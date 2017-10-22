using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnowBLL.Models.Users;
using SnowBLL.Models.Geo;

namespace SnowBLL.Models.Routes
{
    public class RouteDetailsViewModel : RouteViewModel
    {
        public int UserId { get; set; }
        public UserListItemModel User { get; set; }
        public RouteGeometry Geometry { get; set; }
        public GeoPoint MainPoint { get; set; }
    }
}
