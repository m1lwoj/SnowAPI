using SnowBLL.Models;
using SnowBLL.Models.Routes;
using SnowDAL.DBModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnowBLL.Service.Interfaces
{
    public interface IRouteBLService : IBLService
    {
        Task<Result<ListResult<RouteInfo>>> GetAllRoutes(CollectionRequestModel model);
        Task<Result<ListResult<RouteInfo>>> GetRoutesInRange(RoutesInRangeRequestModel model);
        Task<Result<RouteDetailsViewModel>> GetRouteById(IdModel model);
        Task<Result<IEnumerable<RouteGeometry>>> GetGeometries(string routes);
        Task<Result<int>> Create(RouteCreateModel item);
        Task<Result<object>> Update(int id, RouteUpdateModel item);
        Task<Result<object>> Remove(IdModel item, bool instantCommit = true);
        Task<Result<ListResult<RoutePoint>>> GetPoints(PagingRequestModel model, string routes);
    }
}
