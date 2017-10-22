using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models
{
    public class ListResult<TModel>
    {
        public bool HasNext { get; set; }
        public int Count { get; set; }
        public IEnumerable<TModel> Results { get; set; }
    }
}
