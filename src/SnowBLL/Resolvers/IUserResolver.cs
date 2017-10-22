using SnowBLL.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Resolvers
{
    public interface IUserResolver
    {
        string GetEmail();
        Task<bool> IsConfirmed();
        Task<UserModel> GetUser();
    }
}
