using SnowBLL.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SnowBLL.Models;
using SnowDAL.DBModels;
using SnowDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace SnowBLL.Service.Concrete
{
    public class RequestLogService : BLService, IRequestLogService
    {
        private IApiLogRepository _logRepository;

        public RequestLogService(ILogger<BLService> logger, IApiLogRepository logRepository) : base(logger)
        {
            _logRepository = logRepository;
        }

        public void SaveLog(APILogEntity log)
        {
            _logRepository.Add(log);
        }
    }
}
