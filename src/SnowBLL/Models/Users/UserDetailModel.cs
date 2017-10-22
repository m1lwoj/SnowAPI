using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models.Users
{
    public class UserDetailModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public int RoutesCount { get; set; }
        public Dictionary<int, string> Routes{ get; set; }
    }
}
