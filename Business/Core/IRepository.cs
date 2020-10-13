using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Business.Core
{
    public interface IRepository<T>: IDisposable where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(params object[] includes);
        //Task<IEnumerable<T>> GetAsync(params Expression<Func<IQueryable<T>,IIncludableQueryable<T,object>>>[] includes);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, params object[] includes);
        //Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<IQueryable<T>,IIncludableQueryable<T, object>>>[] includes);
        //Task<IQueryable<T>>  GetAsync(string sql, params object[] parameters);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> FindAsync(int id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<T> FindAsync(int id, params object[] includes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null,
             params object[] includes);
        //Task<T> SingleAsync(Expression<Func<T, bool>> predicate = null,
        //    Expression<Func<IQueryable<T>,IOrderedQueryable<T>>>[] orderBy = null,
        //    Expression<Func<IQueryable<T>, IIncludableQueryable<T, object>>>[] includes = null);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Add(params T[] entities);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Add(IEnumerable<T> entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Update(params T[] entities);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void Delete(object id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Delete(params T[] entities);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        void Delete(IEnumerable<T> entities);
    }
}
