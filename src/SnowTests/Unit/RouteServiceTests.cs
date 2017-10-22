using Microsoft.Extensions.Logging;
using Moq;
using SnowBLL.Enums;
using SnowBLL.Models;
using SnowBLL.Models.Routes;
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
using System.Threading.Tasks;
using Xunit;

namespace SnowTests.Unit
{
    public class RouteServiceMocker
    {
        public Mock<IRouteRepository> RepoMock { get; set; }
        public Mock<IRoutePointRepository> RoutePointMock { get; set; }
        public Mock<IUserResolver> UserResolverMock { get; set; }
        public Mock<ILogger<BLService>> LoggerMock { get; set; }

        public RouteServiceMocker()
        {
            RepoMock = new Mock<IRouteRepository>();
            RoutePointMock = new Mock<IRoutePointRepository>();
            UserResolverMock = new Mock<IUserResolver>();
            LoggerMock = new Mock<ILogger<BLService>>();
        }

        public IRouteBLService GetService()
        {
            return new RouteBLService(LoggerMock.Object, RepoMock.Object, UserResolverMock.Object, RoutePointMock.Object);
        }
    }

    public class RouteServiceTests
    {
        [Fact]
        public void UpdateWrongId()
        {
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Update(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test delete"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => null);

            IRouteBLService service = mocker.GetService();

            var result = service.Update(0, new RouteUpdateModel());

            Assert.Equal("Id", result.Result.Error.Errors.First().Field);
            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
        }

        [Fact]
        public void UpdateWithourPermissions()
        {
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Update(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test update"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new RouteInfoEntity() { ID = 1, Name = "test", UserId = 2 });
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));
            mocker.UserResolverMock.Setup(r => r.GetUser())
                  .Returns(() => Task.FromResult<UserModel>(null));
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(false));

            IRouteBLService service = mocker.GetService();

            var result = service.Update(0, new RouteUpdateModel());

            Assert.Equal(ErrorStatus.Forbidden, result.Result.Error.Status);
            Assert.Equal("Action forbidden", result.Result.Error.Message);
        }


        [Fact]
        public void UpdateNotConfirmedUser()
        {
            var mocker = new RouteServiceMocker();

            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Update(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test update"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new RouteInfoEntity() { ID = 1, Name = "test", UserId = 2 });
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));
            mocker.UserResolverMock.Setup(r => r.GetUser())
                    .Returns(() => Task.FromResult<UserModel>(new UserModel()
                    {
                        Email = "test@test.com",
                        Id = 2,
                        Name = "testowy"
                    }));
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(false));

            IRouteBLService service = mocker.GetService();

            var result = service.Update(0, new RouteUpdateModel());

            Assert.Equal(ErrorStatus.Forbidden, result.Result.Error.Status);
            Assert.Equal("User is not confirmed", result.Result.Error.Message);
        }

        [Fact]
        public void UpdateUnhandledException()
        {
            var mocker = new RouteServiceMocker();

            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Update(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test update"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new RouteInfoEntity() { ID = 1, Name = "test", UserId = 2 });
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));
            mocker.UserResolverMock.Setup(r => r.GetUser())
                  .Returns(() => Task.FromResult<UserModel>(new UserModel()
                  {
                      Email = "test@test.com",
                      Id = 2,
                      Name = "testowy"
                  }));
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(true));


            IRouteBLService service = mocker.GetService();

            var result = service.Update(0, new RouteUpdateModel());

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void RemoveInvalidId()
        {
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Delete(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test delete"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => null);

            IRouteBLService service = mocker.GetService();

            var result = service.Update(0, new RouteUpdateModel());

            Assert.Equal("Id", result.Result.Error.Errors.First().Field);
            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
        }

        [Fact]
        public void RemoveWithoutPermissions()
        {
            var mocker = new RouteServiceMocker();
            mocker.RepoMock.Setup(r => r.Delete(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test delete"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new RouteInfoEntity());
            mocker.UserResolverMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(null));
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(false));

            IRouteBLService service = mocker.GetService();

            var result = service.Remove(new IdModel());

            Assert.Equal(ErrorStatus.Forbidden, result.Result.Error.Status);
            Assert.Equal("Action forbidden", result.Result.Error.Message);
        }


        [Fact]
        public void RemoveNotConfirmed()
        {
            var mocker = new RouteServiceMocker();
            mocker.RepoMock.Setup(r => r.Delete(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test delete"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new RouteInfoEntity());
            mocker.UserResolverMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(new UserModel()));
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(false));

            IRouteBLService service = mocker.GetService();

            var result = service.Remove(new IdModel());

            Assert.Equal(ErrorStatus.Forbidden, result.Result.Error.Status);
            Assert.Equal("User is not confirmed", result.Result.Error.Message);
        }

        [Fact]
        public void RemoveUnhandledException()
        {
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Delete(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test delete"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new RouteInfoEntity() { ID = 1, Name = "test", UserId = 2 });
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));
            mocker.UserResolverMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(new UserModel()
            {
                Email = "test@test.com",
                Id = 2,
                Name = "testowy"
            }));
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(true));

            IRouteBLService service = mocker.GetService();

            var result = service.Remove(new IdModel());

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void RemoveValidResult()
        {
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Delete(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test delete"));
            mocker.RepoMock.Setup(r => r.GetSingle(It.IsAny<int>())).Returns(() => new RouteInfoEntity() { ID = 1, Name = "test", UserId = 2 });
            mocker.UserResolverMock.Setup(r => r.GetUser()).Returns(() => Task.FromResult<UserModel>(new UserModel()
            {
                Email = "test@test.com",
                Id = 2,
                Name = "testowy"
            }));
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(true));

            IRouteBLService service = mocker.GetService();

            var result = service.Remove(new IdModel());

            Assert.True(result.Result.IsOk);
        }


        [Fact]
        public void CreateNotConfirmedUser()
        {
            var mocker = new RouteServiceMocker();
            RouteCreateModel model = new RouteCreateModel();
            mocker.RepoMock.Setup(r => r.Add(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test create"));
            mocker.RepoMock.Setup(r => r.IsValidLine(It.IsAny<string>())).Returns(() => false);
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(false));

            IRouteBLService service = mocker.GetService();

            var result = service.Create(new RouteCreateModel());

            Assert.Equal(ErrorStatus.Forbidden, result.Result.Error.Status);
            Assert.Equal("User is not confirmed", result.Result.Error.Message);
        }

        [Fact]
        public void CreateInvalidLine()
        {
            var mocker = new RouteServiceMocker();
            RouteCreateModel model = new RouteCreateModel();
            mocker.RepoMock.Setup(r => r.Add(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test create"));
            mocker.RepoMock.Setup(r => r.IsValidLine(It.IsAny<string>())).Returns(() => false);
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(true));

            IRouteBLService service = mocker.GetService();

            var result = service.Create(new RouteCreateModel());

            Assert.Equal(ErrorStatus.InvalidModel, result.Result.Error.Status);
            Assert.Equal("Invalid route line", result.Result.Error.Message);
        }

        [Fact]
        public void CreateUnhandledException()
        {
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Add(It.IsAny<RouteInfoEntity>())).Callback(() => Console.WriteLine("Unit test create"));
            mocker.RepoMock.Setup(r => r.IsValidLine(It.IsAny<string>())).Returns(() => true);
            mocker.RepoMock.Setup(r => r.Commit()).Throws(new Exception("testexception"));
            mocker.UserResolverMock.Setup(r => r.IsConfirmed()).Returns(() => Task.FromResult<bool>(true));

            IRouteBLService service = mocker.GetService();

            var result = service.Create(new RouteCreateModel());

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void GetGeometriesEmptyRoutes()
        {
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.GetGeometries(It.IsAny<int[]>())).Callback(() => new List<RouteGeomEntity>());

            IRouteBLService service = mocker.GetService();

            var result = service.GetGeometries(string.Empty);

            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
            Assert.Equal("Route not found", result.Result.Error.Message);
        }

        [Fact]
        public void GetGeometriesUnhandledException()
        {
            int[] ids = new int[2] { 32, 42 };
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.GetGeometries(It.IsAny<int[]>())).Throws(new Exception("testexception"));

            IRouteBLService service = mocker.GetService();

            var result = service.GetGeometries("32,42");

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void GetAllRoutesUnhandledException()
        {
            var request = new CollectionRequestModel();
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();
            mocker.RepoMock.Setup(r => r.Search(It.IsAny<SearchQuery<RouteInfoEntity>>())).Throws(new Exception("testexception"));

            IRouteBLService service = mocker.GetService();

            var result = service.GetAllRoutes(request);

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void GetAllRoutesValidResult()
        {
            var request = new CollectionRequestModel();
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();

            mocker.RepoMock.Setup(r => r.Search(It.IsAny<SearchQuery<RouteInfoEntity>>()))
                .Returns(() => Task.FromResult(new PagingResult<RouteInfoEntity>()
                {
                    Count = 2,
                    HasNext = false,
                    Results = new List<RouteInfoEntity>()
                {
                    new RouteInfoEntity()
                    {
                        Difficulty = 2,
                        ID = 1,
                        Name = "First"
                    },
                    new RouteInfoEntity()
                    {
                        Difficulty = 3,
                        ID = 2,
                        Name = "Second"
                    }
                }
                }));

            IRouteBLService service = mocker.GetService();

            var result = service.GetAllRoutes(request);

            Assert.Equal(true, result.Result.IsOk);
            Assert.Equal(2, result.Result.Content.Count);
            Assert.Equal(false, result.Result.Content.HasNext);
        }

        [Fact]
        public void GetRouteByIdUnhandledException()
        {
            var request = new CollectionRequestModel();
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();

            mocker.RepoMock.Setup(r => r.GetSingleWithDependencies(It.IsAny<int>())).Throws(new Exception("testexception"));
            IRouteBLService service = mocker.GetService();
            var result = service.GetRouteById(new IdModel() { Id = 1 });

            Assert.Equal(ErrorStatus.InternalServer, result.Result.Error.Status);
            Assert.Equal("testexception", result.Result.Error.Message);
        }

        [Fact]
        public void GetRouteByIdInvalidId()
        {
            var request = new CollectionRequestModel();
            var mocker = new RouteServiceMocker();
            RouteInfoEntity entity = new RouteInfoEntity();

            mocker.RepoMock.Setup(r => r.GetSingleWithDependencies(It.IsAny<int>())).Returns((() => Task.FromResult((RouteInfoEntity)null)));
            IRouteBLService service = mocker.GetService();
            var result = service.GetRouteById(new IdModel() { Id = 1 });

            Assert.Equal(ErrorStatus.ObjectNotFound, result.Result.Error.Status);
            Assert.Equal("Route not found", result.Result.Error.Message);
        }
    }
}
