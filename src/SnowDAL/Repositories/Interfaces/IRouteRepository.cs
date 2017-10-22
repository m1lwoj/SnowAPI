using SnowDAL.DBModels;
using SnowDAL.Paging;
using SnowDAL.Searching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnowDAL.Repositories.Interfaces
{
    public interface IRouteRepository : IEntityBaseRepository<RouteInfoEntity>
    {
        bool IsValidLine(string line);
        string GenerateGISLine(string line);
        Task<IEnumerable<RouteGeomEntity>> GetGeometries(int[] ids);
        Task<PagingResult<RouteInfoEntity>> Search(SearchQuery<RouteInfoEntity> searchQuery);
        Task<RouteInfoEntity> GetSingleWithDependencies(int id);
        Task<PagingResult<RouteInfoEntity>> GetRoutesInRange(QueryPager pager, string point, int kilometers);
    }
}
