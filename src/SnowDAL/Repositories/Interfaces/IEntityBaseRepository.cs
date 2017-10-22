using SnowDAL.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SnowDAL.Repositories.Interfaces
{
    public interface IEntityBaseRepository<T> where T : IEntityBase
    {
        Task<IEnumerable<T>> GetAll();
        int Count();
        T GetSingle(int id);
        Task<T> GetSingle(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task Commit();
    }
}
