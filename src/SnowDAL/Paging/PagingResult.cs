using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowDAL.Paging
{
    public class PagingResult<TEntity>
    {
        public bool HasNext { get; set; }
        public int Count { get; set; }
        public IEnumerable<TEntity> Results { get; set; }
    }
}
