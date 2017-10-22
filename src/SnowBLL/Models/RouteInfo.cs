using SnowBLL.Enums;

namespace SnowBLL.Models
{
    public class RouteInfo
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public int UserId { get; set; }

        public Difficulty Difficulty { get; set; }
    }
}
