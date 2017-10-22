using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowDAL.Searching
{
    public interface IQueryPaging
    {
        /// <summary>
        /// Number of items to be skipped. Useful for paging.
        /// </summary>
        int Skip { get; set; }

        /// <summary>
        /// Represents the number of items to be returned by the query.
        /// </summary>
        int Take { get; set; }
    }
}
