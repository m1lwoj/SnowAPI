using SnowDAL.DBModels;
using SnowDAL.Paging;
using SnowDAL.Searching;
using System.Threading.Tasks;

namespace SnowDAL.Repositories.Interfaces
{
    public interface IUserRepository : IEntityBaseRepository<UserEntity>
    {
        Task<UserEntity> GetSingleWithDependencies(int id);
        Task<PagingResult<UserEntity>> Search(SearchQuery<UserEntity> query);
    }
}
