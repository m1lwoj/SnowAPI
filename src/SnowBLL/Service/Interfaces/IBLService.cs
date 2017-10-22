using SnowBLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Service.Interfaces
{
    public interface IBLService
    {
        ErrorResult GenerateError(Exception ex);
    }
}
