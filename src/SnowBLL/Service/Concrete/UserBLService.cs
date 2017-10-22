using SnowBLL.Enums;
using SnowBLL.Models;
using SnowBLL.Models.Auth;
using SnowBLL.Models.Users;
using SnowBLL.Resolvers;
using SnowBLL.Service.Interfaces;
using SnowDAL.DBModels;
using SnowDAL.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;
using SnowDAL.Searching;
using SnowDAL.Sorting;
using EmailSender;
using Microsoft.Extensions.Logging;

namespace SnowBLL.Service.Concrete
{
    public class UserBLService : BLService, IUserBLService
    {
        #region Members

        private IUserRepository _userRepository;
        private IRouteRepository _routeRepository;
        private IAuthBLService _authService;
        private IUserResolver _userResolver;
        private ISender _emailSender;
        private ISystemCodeRepository _systemCodeRepository;

        #endregion Members

        #region Constructor

        public UserBLService(ILogger<BLService> logger, IUserRepository repository, IAuthBLService authService, IRouteRepository routeRepository, IUserResolver userResolver, ISender sender, ISystemCodeRepository systemCodeRepository) : base(logger)
        {
            this._userRepository = repository;
            this._authService = authService;
            this._userResolver = userResolver;
            this._routeRepository = routeRepository;
            this._emailSender = sender;
            this._systemCodeRepository = systemCodeRepository;
        }

        #endregion Constructor

        #region Public Methods

        public async Task<Result<ListResult<UserListItemModel>>> GetAllUsers(CollectionRequestModel model)
        {
            try
            {
                var query = new SearchQuery<UserEntity>();

                ApplySorting(model, query);

                ApplyFilters(model, query);

                ApplyPaging(model, query);

                ApplyPaging(model, query);

                var results = await _userRepository.Search(query);

                var listResult = new ListResult<UserListItemModel>()
                {
                    Count = results.Count,
                    HasNext = results.HasNext,
                    Results = results.Results.Select(u => new UserListItemModel() { Name = u.Name, Email = u.Email, Id = u.ID, RoutesCount = u.Routes.Count }).ToList()
                };

                return new Result<ListResult<UserListItemModel>>(listResult);
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<ListResult<UserListItemModel>>(error);
            }
        }

        public async Task<Result<int>> Create(UserCreateModel item)
        {
            try
            {
                UserEntity user = await GetUser(item.Email);

                if (user != null && user.Status == 0)
                {
                    user.CreateDate = DateTime.Now;
                    user.Status = 1;

                    _userRepository.Update(user);

                    await _userRepository.Commit();
                    return new Result<int>(user.ID);
                }
                else if (user == null)
                {
                    UserEntity entity = new UserEntity()
                    {
                        Name = string.IsNullOrEmpty(item.Name) ? item.Email : item.Name,
                        Email = item.Email,
                        HashedPassword = _authService.HashUserPassword(new ApplicationUserModel() { Email = item.Email, Password = item.Password }),
                        CreateDate = DateTime.Now,
                        LastLogin = DateTime.Now,
                        Role = (int)Role.User
                    };

                    _userRepository.Add(entity);

                    await _userRepository.Commit();
                    return new Result<int>(entity.ID);
                }
                else
                {
                    ErrorResult error = GenerateError("User already exists", "Email", "Invalid email address", ErrorStatus.InvalidModel);
                    return new Result<int>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<int>(error);
            }
        }

        public async Task<Result<object>> Remove(IdModel item)
        {
            try
            {
                var entity = await _userRepository.GetSingleWithDependencies(item.Id);

                if (entity != null)
                {
                    if (await CheckUsersPermission(entity))
                    {
                        _userRepository.Delete(entity);

                        foreach (RouteInfoEntity route in entity.Routes)
                        {
                            _routeRepository.Delete(route);
                        }
                    }
                    else
                    {
                        ErrorResult error = GenerateError("Action forbidden", "", "", ErrorStatus.Forbidden);
                        return new Result<object>(error);
                    }

                    await _userRepository.Commit();
                    return new Result<object>();
                }
                else
                {
                    ErrorResult error = GenerateError("User not found", "Id", "Invalid identifier", ErrorStatus.ObjectNotFound);
                    return new Result<object>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<object>(error);
            }
        }

        public async Task<Result<object>> Update(int id, UserUpdateModel item)
        {
            try
            {
                var entity = _userRepository.GetSingle(id);
                if (entity != null)
                {
                    if (await CheckUsersPermission(entity))
                    {
                        entity.Email = item.Email;
                        entity.Name = item.Name;

                        if (!string.IsNullOrEmpty(item.Password))
                        {
                            entity.HashedPassword = _authService.HashUserPassword(new ApplicationUserModel() { Email = item.Email, Password = item.Password });
                        }

                        _userRepository.Update(entity);
                        await _userRepository.Commit();

                        return new Result<object>();
                    }
                    else
                    {
                        ErrorResult error = GenerateError("Action forbidden", "", "", ErrorStatus.Forbidden);
                        return new Result<object>(error);
                    }
                }
                else
                {
                    ErrorResult error = GenerateError("Route not found", "Id", "Invalid identifier", ErrorStatus.ObjectNotFound);
                    return new Result<object>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<object>(error);
            }
        }

        public async Task<UserEntity> GetUser(string email)
        {
            return await _userRepository.GetSingle(u => u.Email == email);
        }

        public async Task<Result<UserDetailModel>> GetById(int id)
        {
            try
            {
                var query = new SearchQuery<UserEntity>();

                UserEntity entity = await _userRepository.GetSingleWithDependencies(id);

                if (entity == null)
                {
                    ErrorResult error = GenerateError("User not found", "Id", "Invalid identifier", ErrorStatus.ObjectNotFound);
                    return new Result<UserDetailModel>(error);
                }

                return new Result<UserDetailModel>(new UserDetailModel()
                {
                    Email = entity.Email,
                    Routes = entity.Routes.ToDictionary(r => r.ID, r => r.Name),
                    Name = entity.Name,
                    RoutesCount = entity.Routes.Count
                });
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<UserDetailModel>(error);
            }
        }

        public UserEntity GetUser(int id)
        {
            return _userRepository.GetSingle(id);
        }

        public async Task<Result<CodeEmailResult>> SendResetPasswordEmail(string email)
        {
            try
            {
                var user = await GetUser(email);

                if (user != null)
                {
                    var newSystemCode = new SystemCodeEntity()
                    {
                        GenerateDate = DateTime.Today,
                        ExpirationDate = DateTime.Today.AddDays(30),
                        Type = (int)SystemCodeType.Reset,
                        UserId = user.ID,
                        Code = GenerateCode()
                    };

                    _systemCodeRepository.Add(newSystemCode);
                    await _systemCodeRepository.Commit();

                    await _emailSender.SendCodeEmail(ContentType.ResetPassword, email, newSystemCode.Code);

                    return new Result<CodeEmailResult>(new CodeEmailResult() { ExpirationDate = newSystemCode.ExpirationDate });
                }
                else
                {
                    ErrorResult error = GenerateError("User not found", "Email", "Invalid email", ErrorStatus.ObjectNotFound);
                    return new Result<CodeEmailResult>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<CodeEmailResult>(error);
            }
        }

        public async Task<Result<ResetPasswordResult>> ResetPassword(NewPasswordModel model)
        {
            try
            {
                var user = await GetUser(model.Email);

                if (user != null)
                {
                    var code = await _systemCodeRepository.GetSingle(sc => sc.UserId == user.ID && sc.Code == model.Code);

                    if (code != null)
                    {
                        if (code.ExpirationDate >= DateTime.Now)
                        {
                            user.HashedPassword = _authService.HashUserPassword(new ApplicationUserModel() { Email = user.Email, Password = model.Password });
                            _userRepository.Update(user);

                            await _userRepository.Commit();

                            return new Result<ResetPasswordResult>();
                        }
                        else
                        {
                            ErrorResult error = GenerateError("Code is after expiration date, regenerate the code", "Code", "Code is after expiration date, regenerate the code", ErrorStatus.ObjectNotFound);
                            return new Result<ResetPasswordResult>(error);
                        }
                    }
                    else
                    {
                        ErrorResult error = GenerateError("Invalid code", "Code", "Invalid code", ErrorStatus.ObjectNotFound);
                        return new Result<ResetPasswordResult>(error);
                    }
                }
                else
                {
                    ErrorResult error = GenerateError("User not found", "Id", "Invalid email", ErrorStatus.ObjectNotFound);
                    return new Result<ResetPasswordResult>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<ResetPasswordResult>(error);
            }
        }

        public async Task<Result<UserConfirmationStatus>> ConfirmAccount(UserAccountConfirmationModel model)
        {
            try
            {
                var user = await _userResolver.GetUser();

                if (user != null)
                {
                    var code = await _systemCodeRepository.GetSingle(sc => sc.UserId == user.Id && sc.Code == model.Code);

                    if (code != null)
                    {
                        if (code.ExpirationDate >= DateTime.Now)
                        {
                            var entity = _userRepository.GetSingle(user.Id);

                            entity.IsConfirmed = true;

                            _userRepository.Update(entity);
                            await _userRepository.Commit();

                            return new Result<UserConfirmationStatus>();
                        }
                        else
                        {
                            ErrorResult error = GenerateError("Code is after expiration date, regenerate the code", "Code", "Code is after expiration date, regenerate the code", ErrorStatus.ObjectNotFound);
                            return new Result<UserConfirmationStatus>(error);
                        }
                    }
                    else
                    {
                        ErrorResult error = GenerateError("Invalid code", "Code", "Invalid code", ErrorStatus.InvalidModel);
                        return new Result<UserConfirmationStatus>(error);
                    }
                }
                else
                {
                    ErrorResult error = GenerateError("User is not logged in", "Id", "", ErrorStatus.Forbidden);
                    return new Result<UserConfirmationStatus>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<UserConfirmationStatus>(error);
            }
        }

        public async Task<Result<CodeEmailResult>> SendConfirmAccountEmail()
        {
            try
            {
                var user = await _userResolver.GetUser();

                if (user != null)
                {
                    var newSystemCode = new SystemCodeEntity()
                    {
                        GenerateDate = DateTime.Today,
                        ExpirationDate = DateTime.Today.AddDays(30),
                        Type = (int)SystemCodeType.Activate,
                        UserId = user.Id,
                        Code = GenerateCode()
                    };

                    _systemCodeRepository.Add(newSystemCode);
                    await _systemCodeRepository.Commit();

                    await _emailSender.SendCodeEmail(ContentType.ActivateAccount, user.Email, newSystemCode.Code);

                    return new Result<CodeEmailResult>(new CodeEmailResult() { ExpirationDate = newSystemCode.ExpirationDate });
                }
                else
                {
                    ErrorResult error = GenerateError("User is not logged in", "Id", "", ErrorStatus.Forbidden);
                    return new Result<CodeEmailResult>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<CodeEmailResult>(error);
            }
        }

        public async Task<Result<UserInfoResult>> GetLoggedUserDetails()
        {
            try
            {
                var user = await _userResolver.GetUser();
                if (user != null)
                {
                    var userInfo = new UserInfoResult() { Email = user.Email, Id = user.Id, IsConfirmed = user.IsConfirmed };
                    return new Result<UserInfoResult>(userInfo);
                }
                else
                {
                    ErrorResult error = GenerateError("User is not logged", "", "", ErrorStatus.Forbidden);
                    return new Result<UserInfoResult>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<UserInfoResult>(error);
            }
        }


        #endregion Public Methods

        #region Private Methods

        private string GenerateCode()
        {
            Random generator = new Random();
            return generator.Next(0, 9999).ToString("D4");
        }

        private async Task<bool> CheckUsersPermission(UserEntity entity)
        {
            var user = await _userResolver.GetUser();
            return user != null && (user.Id == entity.ID || user.Role == Role.Admin);
        }

        private static void ApplyPaging(CollectionRequestModel model, SearchQuery<UserEntity> query)
        {
            int page = model.Page ?? 1;
            int pagesize = model.PageSize ?? 100;

            query.Skip = pagesize * (page - 1);
            query.Take = pagesize;
        }

        private static void ApplySorting(CollectionRequestModel model, SearchQuery<UserEntity> query)
        {
            if (!string.IsNullOrEmpty(model.Sort))
            {
                query.AddSortCriteria(FiltrationHelper.GetSorting<UserEntity>(model.Sort));
            }
            else
            {
                query.AddSortCriteria(new FieldSortOrder<UserEntity>("Name", SortDirection.Ascending));
            }
        }

        private static void ApplyFilters(CollectionRequestModel model, SearchQuery<UserEntity> query)
        {
            if (!string.IsNullOrEmpty(model.Filter))
            {
                var filters = FiltrationHelper.GetFilter<RouteFiltrationModel>(model.Filter);
                query.FiltersDictionary = FiltrationHelper.ConvertToDictionary(filters);
            }
        }

        #endregion Private Methods
    }
}