using SnowDAL.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Service.Interfaces
{
    public interface IRequestLogService : IBLService
    {
        void SaveLog(APILogEntity log);
    }
}
