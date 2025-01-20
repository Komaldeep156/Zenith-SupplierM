using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zenith.Repository.Data;
using Zenith.Repository.DomainModels;

namespace Zenith.Repository.RepositoryFiles
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ZenithDbContext _context;
        private DbSet<T> _entities;
        public IConfiguration _configuration { get; }
       
        public Repository(ZenithDbContext context, IConfiguration IConfiguration)
        {
            _context = context;
            _entities = context.Set<T>();
            _configuration = IConfiguration;
        }

        public IEnumerable<T> GetAll()
        {
            return _entities.AsEnumerable();
        }
        public async Task<T> GetAsync(int id)
        {
            return await _entities.FindAsync(id);
        }
        public async Task<int?> InsertAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");
            _entities.Add(entity);
            _context.Entry(entity).State = EntityState.Added;
            return await _context.SaveChangesAsync();
        }
        public int? Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");
            _entities.Add(entity);
            _context.Entry(entity).State = EntityState.Added;
            return _context.SaveChanges();
        }
        public int? InsertAndGetId(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity is null");
                _entities.Add(entity);
                _context.Entry(entity).State = EntityState.Added;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            //Returns primaryKey value
            var idProperty = entity.GetType().GetProperty("Id").GetValue(entity, null);
            return (int)idProperty;
        }
        public virtual void Add(T entities)
        {
            try
            {
                _context.Set<T>().Add(entities);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException(nameof(entities), "Some thing wrong with your Entered data");
            }
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            try
            {
                _context.Set<T>().AddRange(entities);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ArgumentOutOfRangeException(nameof(entities), "Something went wrong with your entered data.", ex.Message);
            }
        }

        public async Task<int?> InsertMultiAsync(List<T> entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");
            entity.ForEach(a => { _context.Entry(a).State = EntityState.Added; });
            return await _context.SaveChangesAsync();
        }
        public async Task<int?> UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");
            _context.Entry(entity).State = EntityState.Modified;
            return await SaveChangesAsync();
        }
        public async Task<int?> DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");
            _context.Entry(entity).State = EntityState.Deleted;
            _entities.Remove(entity);
            return await SaveChangesAsync();
        }
        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            _context.Entry(entity).State = EntityState.Deleted;
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException(nameof(entities), "The entities collection cannot be null or empty.");

            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Deleted;
            }

            _entities.RemoveRange(entities);
            _context.SaveChanges();
        }


        public async Task<int?> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public int? SaveChanges()
        {
            return _context.SaveChanges();
        }
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }
        public async Task<int?> UpdateAsync(List<T> entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");

            entity.ForEach(a => { _context.Entry(a).State = EntityState.Modified; });
            return await SaveChangesAsync();
        }
        public int? Update(T entity, params Expression<Func<T, object>>[] properties)
        {
            try
            {


                _context.Entry(entity).State = EntityState.Modified;


                return _context.SaveChanges();
                //_context.Dispose();



            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UpdateDbEntryAsync exception: " + ex.Message);
                return 0;
            }
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }
    }
}
