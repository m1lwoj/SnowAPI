using SnowBLL.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnowDAL.DBModels;
using SnowDAL.Repositories.Interfaces;

namespace SnowBLL.Service.Concrete
{
    public class UserResolverService : IUserResolverService
    {
        private IUserRepository _userRepository;

        #region Constructor

        public UserResolverService(IUserRepository repository)
        {
            this._userRepository = repository;
        }

        #endregion Constructor

        public async Task<UserEntity> GetUser(string email)
        {
            return await _userRepository.GetSingle(u => u.Email == email);
        }
    }
}