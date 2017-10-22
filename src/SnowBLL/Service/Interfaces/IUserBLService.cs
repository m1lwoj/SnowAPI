using SnowBLL.Models;
using SnowBLL.Models.Auth;
using SnowBLL.Models.Users;
using SnowDAL.DBModels;
using System.Threading.Tasks;

namespace SnowBLL.Service.Interfaces
{
    public interface IUserBLService : IBLService
    {
        Task<UserEntity> GetUser(string email);
        UserEntity GetUser(int id);
        Task<Result<int>> Create(UserCreateModel item);
        Task<Result<object>> Update(int id, UserUpdateModel item);
        Task<Result<object>> Remove(IdModel item);
        Task<Result<ListResult<UserListItemModel>>> GetAllUsers(CollectionRequestModel collectionRequestModel);
        Task<Result<UserDetailModel>> GetById(int id);
        Task<Result<CodeEmailResult>> SendResetPasswordEmail(string email);
        Task<Result<ResetPasswordResult>> ResetPassword(NewPasswordModel model);
        Task<Result<CodeEmailResult>> SendConfirmAccountEmail();
        Task<Result<UserConfirmationStatus>> ConfirmAccount(UserAccountConfirmationModel model);
        Task<Result<UserInfoResult>> GetLoggedUserDetails();
    }
}
