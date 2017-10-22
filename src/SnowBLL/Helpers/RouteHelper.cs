using System.Linq;

namespace SnowBLL.Helpers
{
    public static class RouteHelper
    {
        public static string GetFirstPoint(string line)
        {
            return line.Split(',').First();
        }
    }
}
