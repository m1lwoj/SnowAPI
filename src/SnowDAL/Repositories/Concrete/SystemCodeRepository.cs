using System.Linq;
using SnowDAL.DBModels;
using SnowDAL.Repositories.Interfaces;


namespace SnowDAL.Repositories.Concrete
{
    public class SystemCodeRepository : EntityBaseRepository<SystemCodeEntity>, ISystemCodeRepository
    {
        public SystemCodeRepository(EFContext context) : base(context)
        {
        }

        public SystemCodeEntity GetLastCode(int type, int userId)
        {
            return _context.SystemCodes
                .Where(sc => sc.UserId == userId && sc.Type == type)
                .OrderByDescending(sc => sc.GenerateDate).
                FirstOrDefault();
        }
    }
}

