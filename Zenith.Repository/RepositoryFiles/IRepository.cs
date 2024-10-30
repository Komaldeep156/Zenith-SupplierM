using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Zenith.Repository.RepositoryFiles
{
    public interface IRepository<T> where T : class
    {
        int? Update(T entity, params Expression<Func<T, object>>[] properties);
        int? InsertAndGetId(T entity);
        void Add(T entities);
        IEnumerable<T> GetAll();
        Task<T> GetAsync(int id);
        Task<int?> InsertAsync(T entity);
        int? Insert(T entity);
        Task<int?> UpdateAsync(T entity);
        Task<int?> DeleteAsync(T entity);
        void Remove(T entity);
        Task<int?> SaveChangesAsync();
        int? SaveChanges();
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        Task<int?> UpdateAsync(List<T> entity);
        Task<int?> InsertMultiAsync(List<T> entity);
        IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties);
    }
}
