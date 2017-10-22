using Microsoft.AspNetCore.Http;
using SnowBLL.Enums;
using SnowBLL.Models.Users;
using SnowBLL.Resolvers;
using SnowBLL.Service.Interfaces;
using SnowDAL.DBModels;
using System.Threading.Tasks;

namespace SnowAPI.Infrastracture
{
    /// <summary>
    /// Logged user resolver
    /// </summary>
    public class UserResolver : IUserResolver
    {
        private readonly IHttpContextAccessor _context;
        private readonly IUserResolverService _userResolverService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">Http context accessor</param>
        /// <param name="userResolverService">User resolver service</param>
        public UserResolver(IHttpContextAccessor context, IUserResolverService userResolverService)
        {
            _context = context;
            _userResolverService = userResolverService;
        }

        /// <summary>
        /// Get logged user email
        /// </summary>
        /// <returns>User's email</returns>
        public string GetEmail()
        {
            return _context.HttpContext.User?.Identity?.Name;
        }

        /// <summary>
        /// Get logged user
        /// </summary>
        /// <returns>User</returns>
        public async Task<UserModel> GetUser()
        {
            string email = GetEmail();
            if (!string.IsNullOrEmpty(email))
            {
                UserEntity entity = await _userResolverService.GetUser(email);
                return GetModel(entity);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check if user is has been confirmed
        /// </summary>
        /// <returns>Confirmation status</returns>
        public async Task<bool> IsConfirmed()
        {
            string email = GetEmail();
            if (!string.IsNullOrEmpty(email))
            {
                var user = await _userResolverService.GetUser(email);
                return user.IsConfirmed || user.Role == (int)Role.Admin;
            }

            return false;
        }

        private UserModel GetModel(UserEntity entity)
        {
            return new UserModel()
            {
                CreateDate = entity.CreateDate,
                Email = entity.Email,
                LastLogin = entity.LastLogin,
                Name = entity.Name,
                Id = entity.ID,
                Password = entity.HashedPassword,
                Role = (Role)entity.Role,
                IsConfirmed = entity.IsConfirmed
            };
        }
    }
}