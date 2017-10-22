using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SnowDAL.DBModels;
using SnowDAL.Paging;
using SnowDAL.Repositories.Interfaces;
using SnowDAL.Searching;
using SnowDAL.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SnowDAL.Extensions;

namespace SnowDAL.Repositories.Concrete
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T>
    where T : class, IEntityBase, new()
    {
        protected EFContext _context;

        public EntityBaseRepository(EFContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return  _context.Set<T>().Where(x => x.Status == 1).AsEnumerable();
        }

        public T GetSingle(int id)
        {
            return _context.Set<T>().FirstOrDefault(x => x.ID == id && x.Status == 1);
        }

        public void Add(T entity)
        {
            entity.Status = 1;
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            _context.Set<T>().Add(entity);
        }

        public virtual void Update(T entity)
        {
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            entity.Status = 0;
            EntityEntry dbEntityEntry = _context.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Executes the query against the repository (database).
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual async Task<PagingResult<T>> GetTheResult(IQueryPaging searchQuery, IQueryable<T> sequence)
        {
            var resultCount = sequence.Count();

            var result = (searchQuery.Take > 0)
                                ? await (sequence.Skip(searchQuery.Skip).Take(searchQuery.Take).ToListAsync())
                                : await (sequence.ToListAsync());

            bool hasNext = (searchQuery.Skip <= 0 && searchQuery.Take <= 0) ? false : (searchQuery.Skip + (searchQuery.Take - 1) < resultCount);

            return new PagingResult<T>()
            {
                Count = resultCount,
                HasNext = hasNext,
                Results = result
            };
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Resolves and applies the sorting criteria of the SearchQuery
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual IQueryable<T> ManageSortCriterias(List<ISortCriteria<T>> sortCriterias, IQueryable<T> sequence)
        {
            if (sortCriterias != null && sortCriterias.Count > 0)
            {
                var sortCriteria = sortCriterias[0];
                var orderedSequence = sortCriteria.ApplyOrdering(sequence, false);

                if (sortCriterias.Count > 1)
                {
                    for (var i = 1; i < sortCriterias.Count; i++)
                    {
                        var sc = sortCriterias[i];
                        orderedSequence = sc.ApplyOrdering(orderedSequence, true);
                    }
                }
                sequence = orderedSequence;
            }
            else
            {
                sequence = ((IOrderedQueryable<T>)sequence).OrderBy(x => (true));
            }
            return sequence;
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Chains the where clause to the IQueriable instance.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual IQueryable<T> ManageFilters(Dictionary<string, object> search, IQueryable<T> sequence)
        {
            if (search != null && search.Count > 0)
            {
                foreach (var filterClause in search)
                {
                    sequence = sequence.WhereGeneric<T>(filterClause.Key, filterClause.Value);
                }
            }
            return sequence;
        }

        //-----------------------------------------------------------
        /// <summary>
        /// Implementation of eager-loading. Includes the properties sent as part of the SearchQuery.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected virtual IQueryable<T> ManageIncludeProperties(string includeProperties, IQueryable<T> sequence)
        {
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                var properties = includeProperties.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var includeProperty in properties)
                {
                    sequence = sequence.Include(c => includeProperty);
                }
            }
            return sequence;
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetSingle(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                .Where(x => x.Status == 1)
                .FirstOrDefaultAsync(predicate);
        }
    }
}
