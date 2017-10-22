using SnowBLL.Enums;
using System;

namespace SnowBLL.Models.Users
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastLogin { get; set; }
        public Role Role { get; set; }
        public bool IsAdmin { get; }
        public bool IsUser { get; }
        public bool IsConfirmed { get; set; }
    }
}
