using SnowDAL.DBModels;

namespace SnowDAL.Repositories.Interfaces
{
    public interface ISystemCodeRepository : IEntityBaseRepository<SystemCodeEntity>
    {
        SystemCodeEntity GetLastCode(int type, int userId);
    }
}
