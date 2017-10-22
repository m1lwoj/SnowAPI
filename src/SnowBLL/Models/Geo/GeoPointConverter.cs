using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SnowBLL.Models.Geo
{
    public class GeoPointConverter
    {
        public static IEnumerable<GeoPoint> GetPoints(string line)
        {
            var points = line.Replace("LINESTRING", "")
                .Replace("(", "")
                .Replace(")", "")
                .Split(',')
                .Select(p => new GeoPoint(p));

            return points;
        }

        public static GeoPoint GetPoint(string point)
        {
            return new GeoPoint()
            {
                Lat = decimal.Parse(point.Replace("POINT", "").Replace("(", "").Replace(")", "").Split(' ')[0], CultureInfo.InvariantCulture),
                Lng = decimal.Parse(point.Replace("POINT", "").Replace("(", "").Replace(")", "").Split(' ')[1], CultureInfo.InvariantCulture)
            };
        }
    }
}
