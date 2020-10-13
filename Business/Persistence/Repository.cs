using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Business.Core;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Business.Persistence
{
    public class Repository<T>: IRepository<T> where T:class
    {
        private readonly CoreDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(CoreDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(params object[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include.ToString());
            }

            return query == null ? await _dbSet.ToListAsync() : await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, params object[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include.ToString());
            }

            return query == null ? await _dbSet.Where(predicate).ToListAsync() :
                await query.Where(predicate).ToListAsync();
        }

        public async Task<T> FindAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T> FindAsync(int id, params object[] includes)
        {
            foreach (var include in includes)
            {
                _dbSet.Include(include.ToString());
            }

            return await _dbSet.FindAsync(id);
        }

        public async Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null, params object[] includes)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include.ToString());
            }

            return query == null
                ? await _dbSet.FirstOrDefaultAsync(predicate)
                : await query.FirstOrDefaultAsync(predicate);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Add(params T[] entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Add(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Update(params T[] entities)
        {
            _context.Entry(entities).State = EntityState.Modified;
        }

        public void Update(IEnumerable<T> entities)
        {
            _context.Entry(entities).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Delete(object id)
        {
            var deletedData = _dbSet.Find(id);
            if (deletedData != null)
            {
                _context.Remove(deletedData);
            }
        }

        public void Delete(params T[] entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    
                }
            }

            this._disposed = true;
        }
    }
}
