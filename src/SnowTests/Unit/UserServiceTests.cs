using EmailSender;
using Microsoft.Extensions.Logging;
using Moq;
using SnowBLL.Enums;
using SnowBLL.Models;
using SnowBLL.Models.Auth;
using SnowBLL.Models.Users;
using SnowBLL.Resolvers;
using SnowBLL.Service.Concrete;
using SnowBLL.Service.Interfaces;
using SnowDAL.DBModels;
using SnowDAL.Paging;
using SnowDAL.Repositories.Interfaces;
using SnowDAL.Searching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace SnowTests.Unit
{
    public class UserServiceMocker
    {
        public Mock<IUserRepository> RepoMock { get; set; }
        public Mock<IAuthBLService> AuthServiceMock { get; set; }
        public Mock<IRouteRepository> RouteRepositoryMock { get; set; }
        public Mock<IUserResolver> UserResolverServiceMock { get; set; }
        public Mock<ISender> EmailSenderMock { get; set; }
        public Mock<ISystemCodeRepository> SystemCodeRepositoryMock { get; set; }
        public Mock<ILogger<BLService>> LoggerMock { get; set; }

        public UserServiceMocker()
        {
            RepoMock = new Mock<IUserRepository>();
            AuthServiceMock = new Mock<IAuthBLService>();
            RouteRepositoryMock = new Mock<IRouteRepository>();
            UserResolverServiceMock = new Mock<IUserResolver>();
            EmailSenderMock = new Mock<ISender>();
            SystemCodeRepositoryMock = new Mock<ISystemCodeRepository>();
            LoggerMock = new Mock<ILogger<BLService>>();
        }

        public IUserBLService GetService()
        {
            return new UserBLService(LoggerMock.Object, RepoMock.Object, AuthServiceMock.Object, RouteRepositoryMock.Object, UserResolverServiceMock.Object, EmailSenderMock.Object, SystemCodeRepositoryMock.Object);
        }
    }

    public class UserServiceTests
    {
        [Fact]
        public void UpdateWrongId()
        {
            UserServiceMocker mocker = new UserServiceMocker();

            UserEntity entity = new UserEntity();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => null);

            IUserBLService service = mocker.GetService();

            var result = service.Update(0, new UserUpdateModel());

            Assert.Equal("Id", result.Result.Error.Errors.First().Field);
            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
        }

        [Fact]
        public void UpdateInvalidUserPermissions()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            UserEntity entity = new UserEntity();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new UserEntity()
            {
                Email = "test@test.com",
                ID = 1,
                Name = "testowy"
            });

            mocker.UserResolverServiceMock.Setup(r => r.GetUser())
                .Returns(() => Task.FromResult<UserModel>(new UserModel()
                {
                    Email = "test@test.com",
                    Id = 2,
                    Name = "testowy"
                }));

            IUserBLService service = mocker.GetService();

            var result = service.Update(0, new UserUpdateModel());

            Assert.Equal("Action forbidden", result.Result.Error.Message);
            Assert.Equal(ErrorStatus.Forbidden, result.Result.Error.Status);
        }

        [Fact]
        public void UpdateUnhandledException()
        {
            UserServiceMocker mocker = new UserServiceMocker();

            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new UserEntity()
            {
                Email = "test@test.com",
                ID = 2,
                Name = "testowy"
            });

            mocker.UserResolverServiceMock.Setup(r => r.GetUser()).Returns(
                () => Task.FromResult<UserModel>(new UserModel()
                {
                    Email = "test@test.com",
                    Id = 2,
                    Name = "testowy"
                }));
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));

            IUserBLService service = mocker.GetService();

            var result = service.Update(0, new UserUpdateModel());

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void RemoveWrongId()
        {
            UserServiceMocker mocker = new UserServiceMocker();

            UserEntity entity = new UserEntity();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => null);

            IUserBLService service = mocker.GetService();

            var result = service.Remove(new IdModel());

            Assert.Equal("Id", result.Result.Error.Errors.First().Field);
            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
        }

        [Fact]
        public void RemoveUnhandledException()
        {
            UserServiceMocker mocker = new UserServiceMocker();

            UserEntity entity = new UserEntity();

            mocker.RepoMock.Setup(r => r.GetSingleWithDependencies(It.IsAny<int>())).Returns(() => Task.FromResult<UserEntity>(new UserEntity()
            {
                Email = "test@test.com",
                ID = 2,
                Name = "testowy",
                Routes = new List<RouteInfoEntity>()
                {
                    new RouteInfoEntity()
                    {
                        ID = 1
                    }
                }
            }));

            mocker.UserResolverServiceMock.Setup(r => r.GetUser())
            .Returns(() => Task.FromResult<UserModel>(new UserModel()
            {
                Email = "test@test.com",
                Id = 2,
                Name = "testowy"
            }));

            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));

            IUserBLService service = mocker.GetService();

            var result = service.Remove(new IdModel());

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void CreateUnhandledException()
        {
            UserServiceMocker mocker = new UserServiceMocker();

            UserEntity entity = new UserEntity();
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
              .Returns(() => Task.FromResult<UserEntity>(null));

            IUserBLService service = mocker.GetService();

            var result = service.Create(new UserCreateModel());

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void CreateExistingUser()
        {
            UserServiceMocker mocker = new UserServiceMocker();

            UserEntity entity = new UserEntity();
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
             .Returns(() => Task.FromResult<UserEntity>(new UserEntity() { Status = 1 }));

            IUserBLService service = mocker.GetService();
            var result = service.Create(new UserCreateModel());

            Assert.Equal(ErrorStatus.InvalidModel, result.Result.Error.Status);
            Assert.Equal("User already exists", result.Result.Error.Message);
        }

        [Fact]
        public void GetAllUsersUnhandledException()
        {
            var request = new CollectionRequestModel();
            UserServiceMocker mocker = new UserServiceMocker();

            mocker.RepoMock.Setup(r => r.Search(It.IsAny<SearchQuery<UserEntity>>())).Throws(new Exception("testexception"));

            IUserBLService service = mocker.GetService();

            var result = service.GetAllUsers(request);

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void GetAllUsersValidResult()
        {
            var request = new CollectionRequestModel();
            UserServiceMocker mocker = new UserServiceMocker();

            mocker.RepoMock.Setup(r => r.Search(It.IsAny<SearchQuery<UserEntity>>()))
                .Returns(() => Task.FromResult(new PagingResult<UserEntity>()
                {
                    Count = 2,
                    HasNext = false,
                    Results = new List<UserEntity>()
                {
                    new UserEntity()
                    {
                        Email = "123@123.com",
                        ID = 1,
                        Name = "First",
                        Routes = new List<RouteInfoEntity>()
                        {
                            new RouteInfoEntity()
                            {
                                ID = 1,
                                Name = "a"
                            }
                        }
                    },
                    new UserEntity()
                    {
                        Email = "23@23.com",
                        ID = 2,
                        Name = "Second",
                        Routes = new List<RouteInfoEntity>()
                        {
                            new RouteInfoEntity()
                            {
                                ID = 2,
                                Name = "b"
                            }
                        }
                    }
                }
                }));

            IUserBLService service = mocker.GetService();

            var result = service.GetAllUsers(request);

            Assert.Equal(true, result.Result.IsOk);
            Assert.Equal(2, result.Result.Content.Count);
            Assert.Equal(false, result.Result.Content.HasNext);
        }

        [Fact]
        public void GetUserByIdUnhandledException()
        {
            UserServiceMocker mocker = new UserServiceMocker();

            mocker.RepoMock.Setup(r => r.GetSingleWithDependencies(It.IsAny<int>())).Throws(new Exception("testexception"));

            IUserBLService service = mocker.GetService();

            var result = service.GetById(1);

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void GetRouteByIdInvalidId()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingleWithDependencies(It.IsAny<int>())).Returns((() => Task.FromResult((UserEntity)null)));

            IUserBLService service = mocker.GetService();

            var result = service.GetById(1);

            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
            Assert.Equal("User not found", result.Result.Error.Message);
        }

        [Fact]
        public void SendResetPasswordEmailNullUser()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
             .Returns(() => Task.FromResult<UserEntity>(null));

            IUserBLService service = mocker.GetService();

            var result = service.SendResetPasswordEmail("test@text.com");

            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
            Assert.Equal("User not found", result.Result.Error.Message);
        }

        [Fact]
        public void SendResetPasswordEmailUnhandledException()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(() => Task.FromResult<UserEntity>(new UserEntity()));
            mocker.SystemCodeRepositoryMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));

            IUserBLService service = mocker.GetService();

            var result = service.SendResetPasswordEmail("test@text.com");

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void SendResetPasswordEmailValidResult()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(() => Task.FromResult<UserEntity>(new UserEntity()));

            IUserBLService service = mocker.GetService();

            var result = service.SendResetPasswordEmail("test@text.com");

            Assert.True(result.Result.IsOk);
            Assert.Equal(DateTime.Today.AddDays(30), result.Result.Content.ExpirationDate);
        }

        [Fact]
        public void ResetPasswordlNullUser()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
             .Returns(() => Task.FromResult<UserEntity>(null));

            IUserBLService service = mocker.GetService();

            var result = service.ResetPassword(new NewPasswordModel());

            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
            Assert.Equal("User not found", result.Result.Error.Message);
        }

        [Fact]
        public void ResetPasswordInvalidCode()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(() => Task.FromResult<UserEntity>(new UserEntity()));
            mocker.SystemCodeRepositoryMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<SystemCodeEntity, bool>>>()))
                .Returns(() => Task.FromResult<SystemCodeEntity>(null));

            IUserBLService service = mocker.GetService();

            var result = service.ResetPassword(new NewPasswordModel());

            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
            Assert.Equal("Invalid code", result.Result.Error.Message);
        }

        [Fact]
        public void ResetPasswordUnhandledException()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(() => Task.FromResult<UserEntity>(new UserEntity()));
            mocker.SystemCodeRepositoryMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<SystemCodeEntity, bool>>>()))
                .Returns(() => Task.FromResult<SystemCodeEntity>(new SystemCodeEntity()));
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));

            IUserBLService service = mocker.GetService();

            var result = service.ResetPassword(new NewPasswordModel());

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void ResetPasswordEmailValidResult()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
                .Returns(() => Task.FromResult<UserEntity>(new UserEntity()));
            mocker.SystemCodeRepositoryMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<SystemCodeEntity, bool>>>()))
                .Returns(() => Task.FromResult<SystemCodeEntity>(new SystemCodeEntity()));

            IUserBLService service = mocker.GetService();

            var result = service.SendResetPasswordEmail("test@text.com");

            Assert.True(result.Result.IsOk);
        }

        [Fact]
        public void SendConfirmAccountEmailnullUser()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
             .Returns(() => Task.FromResult<UserEntity>(null));

            IUserBLService service = mocker.GetService();

            var result = service.SendConfirmAccountEmail();

            Assert.Equal(ErrorStatus.Forbidden, result.Result.Error.Status);
            Assert.Equal("User is not logged in", result.Result.Error.Message);
        }

        [Fact]
        public void SendConfirmAccountUnhandledException()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.UserResolverServiceMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(new UserModel()));
            mocker.SystemCodeRepositoryMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));

            IUserBLService service = mocker.GetService();

            var result = service.SendConfirmAccountEmail();

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void SendConfirmAccountEmailValidResult()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.UserResolverServiceMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(new UserModel()));

            IUserBLService service = mocker.GetService();

            var result = service.SendConfirmAccountEmail();

            Assert.True(result.Result.IsOk);
            Assert.Equal(DateTime.Today.AddDays(30), result.Result.Content.ExpirationDate);
        }

        [Fact]
        public void ConfirmAccountEmailNullUser()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<UserEntity, bool>>>()))
             .Returns(() => Task.FromResult<UserEntity>(null));

            IUserBLService service = mocker.GetService();

            var result = service.ConfirmAccount(new UserAccountConfirmationModel());

            Assert.Equal(ErrorStatus.Forbidden, result.Result.Error.Status);
            Assert.Equal("User is not logged in", result.Result.Error.Message);
        }

        [Fact]
        public void ConfirmAccountInvalidCode()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.UserResolverServiceMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(new UserModel()));
            mocker.SystemCodeRepositoryMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<SystemCodeEntity, bool>>>()))
                .Returns(() => Task.FromResult<SystemCodeEntity>(null));

            IUserBLService service = mocker.GetService();

            var result = service.ConfirmAccount(new UserAccountConfirmationModel());

            Assert.Equal(ErrorStatus.InvalidModel, result.Result.Error.Status);
            Assert.Equal("Invalid code", result.Result.Error.Message);
        }

        [Fact]
        public void ConfirmAccountUnhandledException()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.UserResolverServiceMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(new UserModel()));
            mocker.SystemCodeRepositoryMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<SystemCodeEntity, bool>>>()))
             .Returns(() => Task.FromResult<SystemCodeEntity>(new SystemCodeEntity()));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new UserEntity());
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));

            IUserBLService service = mocker.GetService();

            var result = service.ConfirmAccount(new UserAccountConfirmationModel());

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void ConfirmAccountEmailValidResult()
        {
            UserServiceMocker mocker = new UserServiceMocker();
            mocker.UserResolverServiceMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(new UserModel()));
            mocker.SystemCodeRepositoryMock.Setup(r => r.GetSingle(It.IsAny<Expression<Func<SystemCodeEntity, bool>>>()))
             .Returns(() => Task.FromResult<SystemCodeEntity>(new SystemCodeEntity()));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new UserEntity());

            IUserBLService service = mocker.GetService();

            var result = service.ConfirmAccount(new UserAccountConfirmationModel());

            Assert.True(result.Result.IsOk);
        }

    }
}
