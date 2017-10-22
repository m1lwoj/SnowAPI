using SnowDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnowDAL.DBModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace SnowDAL.Repositories.Concrete
{
    public class ApiLogRepository : IApiLogRepository
    {
        protected EFContext _context;

        public ApiLogRepository(EFContext context)
        {
            _context = context;
        }

        public void Add(APILogEntity entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<APILogEntity>(entity);
            _context.Logs.Add(entity);

            _context.SaveChanges();
        }
    }
}
