using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models.Users
{
    public class UserListItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int RoutesCount { get; set; }
    }
}
