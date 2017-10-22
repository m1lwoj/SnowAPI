using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SnowDAL.Sorting
{
    //-----------------------------------------------------------------------
    /// <summary>
    /// Common interface to the sort implementations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISortCriteria<T>
    {
        //-----------------------------------------------------------------------
        SortDirection Direction { get; set; }

        //-----------------------------------------------------------------------
        IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query, Boolean useThenBy);
    }
}
