using SnowBLL.Models.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowBLL.Models
{
    /// <summary>
    /// Collection request model
    /// </summary>
    public class CollectionRequestModel : IPagingRequestModel
    {
        public CollectionRequestModel()
        {

        }

        public CollectionRequestModel(int? page, int? pagesize, string sort, string filter)
        {
            Page = page;
            PageSize = pagesize;
            Sort = sort;
            Filter = filter;
        }

        /// <summary>
        /// Page
        /// </summary>
        public int? Page { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Sorting filed e.g name (Ascending), -name (desceding)
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// Filter query, e.g name::newline|difficulty::4
        /// </summary>
        public string Filter { get; set; }
    }
}
