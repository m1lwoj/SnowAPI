using SnowDAL.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Service.Interfaces
{
    public interface IUserResolverService
    {
        Task<UserEntity> GetUser(string email);
    }
}
