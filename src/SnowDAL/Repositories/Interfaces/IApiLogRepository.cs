using SnowDAL.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowDAL.Repositories.Interfaces
{
    public interface IApiLogRepository
    {
        void Add(APILogEntity entity);
    }
}
