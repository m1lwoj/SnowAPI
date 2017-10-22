using SnowBLL.Enums;
using SnowBLL.Models;
using SnowBLL.Models.Auth;
using System.Threading.Tasks;

namespace SnowBLL.Service.Interfaces
{
    public interface IAuthBLService : IBLService
    {
        Task<Result<AuthorizeResponseModel>> Authorize(ApplicationUserModel model);
        Task<Role> GetRole(string email);
        string HashUserPassword(ApplicationUserModel model);
    }
}
