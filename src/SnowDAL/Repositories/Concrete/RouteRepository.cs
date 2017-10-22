using Microsoft.EntityFrameworkCore;
using SnowDAL.DBModels;
using SnowDAL.Repositories.Concrete;
using SnowDAL.Repositories.Interfaces;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SnowDAL.Searching;
using SnowDAL.Extensions;
using SnowDAL.Paging;
using System.Data;
using Npgsql;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SnowDAL.Concrete.Repositories
{
    public class RouteRepository : EntityBaseRepository<RouteInfoEntity>, IRouteRepository
    {
        public RouteRepository(EFContext context) : base(context)
        {
        }

        public string GenerateGISLine(string line)
        {
            return "a";
        }

        public async Task<RouteInfoEntity> GetSingleWithDependencies(int id)
        {
            return await this._context.Routes
                .Include(r => r.Geometry)
                .Include(r => r.User)
                .Include(r => r.Point)
                .FirstOrDefaultAsync(r => r.ID == id && r.Status == 1);
        }

        public bool IsValidLine(string line)
        {
            try
            {
                bool result = false;

                using (NpgsqlConnection conn = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
                {
                    conn.Open();
                    NpgsqlCommand command = new NpgsqlCommand(RouteGeomEntityProcedures.ISVALIDROUTE_PROCEDURE, conn);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@line", line);
                    NpgsqlDataReader dr = command.ExecuteReader();

                    while (dr.Read())
                        result = (bool)dr[0];

                    dr.Close();
                    conn.Close();
                }

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<RouteGeomEntity>> GetGeometries(int[] ids)
        {
            return await this._context.RoutesGeom.Where(route => ids.Any(id => id == route.RouteInfo.ID && route.RouteInfo.Status == 1)).ToListAsync();
        }

        public async Task<PagingResult<RouteInfoEntity>> Search(SearchQuery<RouteInfoEntity> searchQuery)
        {
            IQueryable<RouteInfoEntity> sequence = this._context.Routes;

            sequence = ManageFilters(searchQuery.FiltersDictionary, sequence);

            sequence = ManageIncludeProperties(searchQuery.IncludeProperties, sequence);

            sequence = ManageSortCriterias(searchQuery.SortCriterias, sequence);

            return await GetTheResult(searchQuery, sequence);
        }

        public async Task<PagingResult<RouteInfoEntity>> GetRoutesInRange(QueryPager pager, string point, int kilometers)
        {
            List<int> ids = new List<int>(pager.Take);
            long count = 0;

            using (NpgsqlConnection conn = new NpgsqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                conn.Open();
                var getInRangeCountCommand = new NpgsqlCommand(RouteGeomEntityProcedures.GETINRANGECOUNT_PROCEDURE, conn);
                getInRangeCountCommand.CommandType = CommandType.StoredProcedure;
                getInRangeCountCommand.Parameters.AddWithValue(@"point", point);
                getInRangeCountCommand.Parameters.AddWithValue(@"dist", kilometers);
                NpgsqlDataReader countDataReader = getInRangeCountCommand.ExecuteReader();

                while (countDataReader.Read())
                    count = (long)countDataReader[0];
                countDataReader.Close();

                var getInRangeCommand = new NpgsqlCommand(RouteGeomEntityProcedures.GETINRANGE_PROCEDURE, conn);
                getInRangeCommand.CommandType = CommandType.StoredProcedure;
                getInRangeCommand.Parameters.AddWithValue("@point", point);
                getInRangeCommand.Parameters.AddWithValue("@dist", kilometers);
                getInRangeCommand.Parameters.AddWithValue("@take", pager.Take);
                getInRangeCommand.Parameters.AddWithValue("@skip", pager.Skip);
                var rangeDataReader = getInRangeCommand.ExecuteReader();

                while (rangeDataReader.Read())
                    ids.Add((int)rangeDataReader[0]);
                rangeDataReader.Close();

                conn.Close();
            }

            return new PagingResult<RouteInfoEntity>()
            {
                Count = (int)count,
                HasNext = (pager.Skip <= 0 && pager.Take <= 0) ? false : (pager.Skip + (pager.Take - 1) < count),
                Results = await _context.Routes.Where(r => ids.Any(id => id == r.ID)).ToListAsync()
            };
        }

        public override void Delete(RouteInfoEntity entity)
        {
            entity.Geometry.Status = 0;
            EntityEntry geomEntity = _context.Entry(entity.Geometry);
            geomEntity.State = EntityState.Modified;

            entity.Point.Status = 0;
            EntityEntry pointEntity = _context.Entry(entity.Point);
            pointEntity.State = EntityState.Modified;

            base.Delete(entity);
        }
    }
}
