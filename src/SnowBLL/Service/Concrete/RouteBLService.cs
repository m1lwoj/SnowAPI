using System;
using System.Collections.Generic;
using SnowBLL.Service.Interfaces;
using SnowDAL.DBModels;
using SnowDAL.Repositories.Interfaces;
using System.Linq;
using SnowBLL.Models;
using System.Threading.Tasks;
using SnowBLL.Enums;
using SnowBLL.Models.Routes;
using SnowDAL.Searching;
using SnowDAL.Sorting;
using SnowBLL.Resolvers;
using SnowBLL.Helpers;
using SnowBLL.Models.Geo;
using Microsoft.Extensions.Logging;
using System.Globalization;
using SnowBLL.Models.Users;

namespace SnowBLL.Service.Concrete
{
    public class RouteBLService : BLService, IRouteBLService
    {
        #region Members

        private IRouteRepository _routeRepository;
        private IUserResolver _userResolver;
        private IRoutePointRepository _pointRepository;

        #endregion Members

        #region Constructor

        public RouteBLService(ILogger<BLService> logger, IRouteRepository repository, IUserResolver userResolver, IRoutePointRepository pointRepository) : base(logger)
        {
            _routeRepository = repository;
            _userResolver = userResolver;
            _pointRepository = pointRepository;
        }

        #endregion Constructor

        #region Public Methods

        public async Task<Result<RouteDetailsViewModel>> GetRouteById(IdModel model)
        {
            try
            {
                var query = new SearchQuery<RouteInfoEntity>();

                RouteInfoEntity result = await _routeRepository.GetSingleWithDependencies(model.Id);

                if (result == null)
                {
                    ErrorResult error = GenerateError("Route not found", "Id", "Invalid identifier", ErrorStatus.ObjectNotFound);
                    return new Result<RouteDetailsViewModel>(error);
                }

                var viewModel = new RouteDetailsViewModel()
                {
                    Difficulty = result.Difficulty,
                    Id = result.ID,
                    Name = result.Name,
                    UserId = result.UserId,
                    User = new UserListItemModel()
                    {
                        Email = result.User.Email,
                        Id = result.User.ID,
                        Name = result.User.Name,
                        RoutesCount = result.User.Routes.Count
                    },
                    Geometry = new RouteGeometry()
                    {
                        Id = result.Geometry.ID,
                        Points = GeoPointConverter.GetPoints(result.Geometry.Line),
                    },
                    MainPoint = GeoPointConverter.GetPoint(result.Point.Point)
                };

                return new Result<RouteDetailsViewModel>(viewModel);
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<RouteDetailsViewModel>(error);
            }
        }

        public async Task<Result<ListResult<RouteInfo>>> GetAllRoutes(CollectionRequestModel model)
        {
            try
            {
                var query = new SearchQuery<RouteInfoEntity>();

                ApplySorting(model, query);

                ApplyFilters(model, query);

                ApplyPaging(model, query);

                var results = await _routeRepository.Search(query);

                var listResult = new ListResult<RouteInfo>()
                {
                    Count = results.Count,
                    HasNext = results.HasNext,
                    Results = results.Results.Select(r => new RouteInfo() { Id = r.ID, Name = r.Name, Difficulty = (Difficulty)r.Difficulty, UserId = r.UserId }).ToList()
                };

                return new Result<ListResult<RouteInfo>>(listResult);
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<ListResult<RouteInfo>>(error);
            }
        }

        public async Task<Result<ListResult<RouteInfo>>> GetRoutesInRange(RoutesInRangeRequestModel model)
        {
            try
            {
                var query = new QueryPager();

                ApplyPaging(model, query);
                string point = $"POINT({model.Point.Lng} {model.Point.Lat})";

                var results = await _routeRepository.GetRoutesInRange(query, point, model.Kilometers);

                var listResult = new ListResult<RouteInfo>()
                {
                    Count = results.Count,
                    HasNext = results.HasNext,
                    Results = results.Results.Select(r => new RouteInfo() { Id = r.ID, Name = r.Name, Difficulty = (Difficulty)r.Difficulty, UserId = r.UserId }).ToList()
                };

                return new Result<ListResult<RouteInfo>>(listResult);
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<ListResult<RouteInfo>>(error);
            }
        }

        public async Task<Result<IEnumerable<RouteGeometry>>> GetGeometries(string routes)
        {
            try
            {
                if (!string.IsNullOrEmpty(routes))
                {
                    var routesArr = routes.Split(',').Select<string, int>(int.Parse).ToArray();

                    var entities = await _routeRepository.GetGeometries(routesArr);

                    if (entities.Any())
                    {
                        var models = entities.Select(r => new RouteGeometry()
                        {
                            Id = r.ID,
                            Points = GeoPointConverter.GetPoints(r.Line)
                        });
                        return new Result<IEnumerable<RouteGeometry>>(models);
                    }
                    else
                    {
                        ErrorResult error = GenerateError("Routes not found", "Routes", "Invalid identifiers", ErrorStatus.ObjectNotFound);
                        return new Result<IEnumerable<RouteGeometry>>(error);
                    }
                }
                else
                {
                    ErrorResult error = GenerateError("Route not found", "Routes", "Invalid identifier", ErrorStatus.ObjectNotFound);
                    return new Result<IEnumerable<RouteGeometry>>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<IEnumerable<RouteGeometry>>(error);
            }
        }

        public async Task<Result<ListResult<RoutePoint>>> GetPoints(PagingRequestModel model, string routes)
        {
            try
            {
                var pager = new QueryPager();

                ApplyPaging(model, pager);

                var results = await _pointRepository.Search(pager);

                var listResult = new ListResult<RoutePoint>()
                {
                    Count = results.Count,
                    HasNext = results.HasNext,
                    Results = results.Results.Select(r => new RoutePoint()
                    {
                        Id = r.RouteInfo.ID,
                        Point = GeoPointConverter.GetPoint(r.Point)
                    }).ToList()
                };

                return new Result<ListResult<RoutePoint>>(listResult);
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<ListResult<RoutePoint>>(error);
            }
        }

        public async Task<Result<int>> Create(RouteCreateModel item)
        {
            try
            {
                if (await _userResolver.IsConfirmed())
                {
                    string line = $"LINESTRING({item.Line})";

                    bool isValidLine = _routeRepository.IsValidLine(line);

                    if (isValidLine)
                    {
                        string point = RouteHelper.GetFirstPoint(item.Line);

                        var user = await _userResolver.GetUser();

                        var infoEntity = new RouteInfoEntity()
                        {
                            Difficulty = item.Difficulty,
                            Name = item.Name,
                            Geometry = new RouteGeomEntity()
                            {
                                Line = line,
                                Status = 1
                            },
                            UserId = user.Id,
                            Point = new RoutePointEntity()
                            {
                                Point = $"POINT({point})",
                                Status = 1
                            }
                        };

                        _routeRepository.Add(infoEntity);

                        await _routeRepository.Commit();
                        return new Result<int>(infoEntity.ID);
                    }
                    else
                    {
                        ErrorResult error = GenerateError("Invalid route line", "Line", "Invalid geometry", ErrorStatus.InvalidModel);
                        return new Result<int>(error);
                    }
                }
                else
                {
                    ErrorResult error = GenerateError("User is not confirmed", "", "", ErrorStatus.Forbidden);
                    return new Result<int>(error);
                }
            }
            catch (Exception ex)
            {
                ErrorResult error = GenerateError(ex);
                return new Result<int>(error);
            }
        }

        public async Task<Result<object>> Update(int id, RouteUpdateModel item)
        {
            try
            {
                var entity = _routeRepository.GetSingle(id);
                if (entity != null)
                {
                    if (await CheckUsersPermission(entity))
                    {
                        if (await _userResolver.IsConfirmed())
                        {
                            entity.Difficulty = item.Difficulty;
                            entity.Name = item.Name;

                            _routeRepository.Update(entity);
                            await _routeRepository.Commit();

                            return new Result<object>();
                        }
                        else
                        {
                            ErrorResult error = GenerateError("User is not confirmed", "", "", ErrorStatus.Forbidden);
                            return new Result<object>(error);
                        }
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

        public async Task<Result<object>> Remove(IdModel item, bool instantCommit = true)
        {
            try
            {
                var entity = _routeRepository.GetSingle(item.Id);

                if (entity != null)
                {
                    if (await CheckUsersPermission(entity))
                    {
                        if (await _userResolver.IsConfirmed())
                        {
                            _routeRepository.Delete(entity);

                            if (entity.Point != null)
                            {
                                entity.Point.Status = 1;
                            }
                            if (entity.Geometry != null)
                            {
                                entity.Geometry.Status = 1;
                            }

                            if (instantCommit)
                            {
                                await _routeRepository.Commit();
                            }

                            return new Result<object>();
                        }
                        else
                        {
                            ErrorResult error = GenerateError("User is not confirmed", "", "", ErrorStatus.Forbidden);
                            return new Result<object>(error);
                        }
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

        #endregion Public Methods

        #region Private Methods

        private static void ApplyPaging(IPagingRequestModel model, IQueryPaging query)
        {
            int page = model.Page ?? 1;
            int pagesize = model.PageSize ?? 100;

            query.Skip = pagesize * (page - 1);
            query.Take = pagesize;
        }

        private static void ApplySorting(CollectionRequestModel model, SearchQuery<RouteInfoEntity> query)
        {
            if (!string.IsNullOrEmpty(model.Sort))
            {
                query.AddSortCriteria(FiltrationHelper.GetSorting<RouteInfoEntity>(model.Sort));
            }
            else
            {
                query.AddSortCriteria(new FieldSortOrder<RouteInfoEntity>("Name", SortDirection.Ascending));
            }
        }

        private static void ApplyFilters(CollectionRequestModel model, SearchQuery<RouteInfoEntity> query)
        {
            if (!string.IsNullOrEmpty(model.Filter))
            {
                var filters = FiltrationHelper.GetFilter<RouteFiltrationModel>(model.Filter);
                query.FiltersDictionary = FiltrationHelper.ConvertToDictionary(filters);
            }
        }

        private async Task<bool> CheckUsersPermission(RouteInfoEntity entity)
        {
            var user = await _userResolver.GetUser();
            return user != null && (user.Id == entity.UserId || user.Role == Role.Admin);
        }

        #endregion
    }
}