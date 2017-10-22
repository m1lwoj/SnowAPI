using Microsoft.EntityFrameworkCore;
using SnowDAL.DBModels;
using SnowDAL.Extensions;
using SnowDAL.Repositories.Interfaces;
using System.Threading.Tasks;
using SnowDAL.Searching;
using System;
using SnowDAL.Paging;
using System.Linq;

namespace SnowDAL.Repositories.Concrete
{
    public class UserRepository : EntityBaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(EFContext context) : base(context)
        {
        }

        public async Task<UserEntity> GetSingleWithDependencies(int id)
        {
            return await this._context.Users
                .Include(u => u.Routes)
                .FirstOrDefaultAsync(x => x.ID == id && x.Status == 1);
        }

        /// <summary>
        /// Search method converting search query to SQL
        /// </summary>
        /// <param name="searchQuery">Search parameters</param>
        /// <returns>Paging result containing user entities</returns>
        public async Task<PagingResult<UserEntity>> Search(SearchQuery<UserEntity> searchQuery)
        {
            IQueryable<UserEntity> sequence = this._context.Users.Include(u => u.Routes);

            sequence = ManageFilters(searchQuery.FiltersDictionary, sequence);

            sequence = ManageIncludeProperties(searchQuery.IncludeProperties, sequence);

            sequence = ManageSortCriterias(searchQuery.SortCriterias, sequence);

            return await GetTheResult(searchQuery, sequence);
        }
    }
}
