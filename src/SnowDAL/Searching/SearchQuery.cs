﻿using SnowDAL.Sorting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SnowDAL.Searching
{
    /// <typeparam name="TEntity"></typeparam>
    public class SearchQuery<TEntity> : IQueryPaging
    {
        //-----------------------------------------------------------------------
        /// <summary>
        /// Default constructor
        /// </summary>
        public SearchQuery()
        {
            SortCriterias = new List<ISortCriteria<TEntity>>();
            FiltersDictionary = new Dictionary<string, object>();
        }

        //-----------------------------------------------------------------------
        /// <summary>
        /// Contains a list of filters to be applied to the query.
        /// </summary>
        public Dictionary<string, object> FiltersDictionary { get; set; }

        //-----------------------------------------------------------------------
        /// <summary>
        /// Contains a list of criterias that would be used for sorting.
        /// </summary>
        public List<ISortCriteria<TEntity>> SortCriterias
        {
            get;
            protected set;
        }

        //-----------------------------------------------------------------------
        /// <summary>
        /// Adds a Sort Criteria to the list.
        /// </summary>
        /// <param name="sortCriteria"></param>
        public void AddSortCriteria(ISortCriteria<TEntity> sortCriteria)
        {
            SortCriterias.Add(sortCriteria);
        }

        //-----------------------------------------------------------------------
        /// <summary>
        /// Contains a list of properties that would be eagerly loaded 
        /// with he query.
        /// </summary>
        public string IncludeProperties { get; set; }

        //-----------------------------------------------------------------------
        /// <summary>
        /// Number of items to be skipped. Useful for paging.
        /// </summary>
        public int Skip { get; set; }

        //-----------------------------------------------------------------------
        /// <summary>
        /// Represents the number of items to be returned by the query.
        /// </summary>
        public int Take { get; set; }
    }
}