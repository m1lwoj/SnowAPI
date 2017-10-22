using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Enums
{
    public enum ErrorStatus
    {
        InternalServer = 1,
        ObjectNotFound = 2,
        InvalidModel = 3,
        Forbidden = 4
    }
}
