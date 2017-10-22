using SnowDAL.DBModels;
using SnowDAL.Paging;
using SnowDAL.Searching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SnowDAL.Repositories.Interfaces
{
    public interface IRoutePointRepository : IEntityBaseRepository<RoutePointEntity>
    {
        Task<PagingResult<RoutePointEntity>> Search(IQueryPaging pager);
    }
}
