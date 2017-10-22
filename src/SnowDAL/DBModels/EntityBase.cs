using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowDAL.DBModels
{
    public class EntityBase : IEntityBase
    {
        public int ID
        {
            get; set;
        }

        public int Status
        {
            get; set;
        }
    }
}
