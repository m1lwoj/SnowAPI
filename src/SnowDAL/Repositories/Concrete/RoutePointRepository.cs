using SnowDAL.DBModels;
using SnowDAL.Repositories.Interfaces;
using System.Threading.Tasks;
using SnowDAL.Paging;
using SnowDAL.Searching;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SnowDAL.Repositories.Concrete
{
    public class RoutePointRepository : EntityBaseRepository<RoutePointEntity>, IRoutePointRepository
    {
        public RoutePointRepository(EFContext context) : base(context)
        {
        }

        public async Task<PagingResult<RoutePointEntity>> Search(IQueryPaging pager)
        {
            IQueryable<RoutePointEntity> sequence = this._context.RoutesPoint.Include(rp => rp.RouteInfo).Where(rp => rp.Status == 1);

            return await GetTheResult(pager, sequence);
        }
    }
}
