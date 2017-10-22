﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SnowDAL.Sorting
{
    public class ExpressionSortCriteria<T, TKey> : ISortCriteria<T>
    {
        //-----------------------------------------------------------------------
        public Expression<Func<T, TKey>> SortExpression { get; set; }

        //-----------------------------------------------------------------------
        public SortDirection Direction { get; set; }

        //-----------------------------------------------------------------------
        public ExpressionSortCriteria()
        {
            Direction = SortDirection.Ascending;
        }

        //-----------------------------------------------------------------------
        /// <summary>
        /// If the "name" value has an " asc" or " desc" on the end, 
        /// the Direction property will be set accordinly.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="useThenBy"></param>
        public IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query, Boolean useThenBy)
        {
            IOrderedQueryable<T> result = null;
            if (SortExpression != null)
            {
                if (Direction == SortDirection.Ascending)
                {
                    result = !useThenBy ? query.OrderBy(SortExpression) : ((IOrderedQueryable<T>)query).ThenBy(SortExpression);
                }
                else
                {
                    result = !useThenBy ? query.OrderByDescending(SortExpression) : ((IOrderedQueryable<T>)query).ThenBy(SortExpression);
                }
            }
            else
            {
                return query.OrderBy(x => x);
            }
            return result;
        }
    }
}