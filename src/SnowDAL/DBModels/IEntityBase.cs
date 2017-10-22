using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowDAL.DBModels
{
    public interface IEntityBase
    {
        int ID { get; set; }
        int Status { get; set; }
    }
}
