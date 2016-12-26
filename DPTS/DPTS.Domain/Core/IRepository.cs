using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DPTS.Domain.Core
{
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Add new entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Task</returns>
        void AddAsync(TEntity entity);

        /// <summary>
        /// Update existing entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Task</returns>
        void UpdateAsync(TEntity entity);

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entity">Entity</param>
        Task RemoveAsync(TEntity entity);

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Task<Entity></returns>
        Task<TEntity> GetByIdAsync(int id);

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="enableTracking">Enable Entity Tracking. Default is true</param>
        /// <returns>Task<IEnumerable<TEntity>></returns>
        Task<IEnumerable<TEntity>> GetAllAsync(bool enableTracking = true);

        /// <summary>
        /// Find entities matching the specified condition
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Task<IEnumerable<TEntity>></returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Find only single record matching the specified condition
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Task<TEntity></returns>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Return true if records for the specified condition exists
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Task<Bool></returns>
        Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<TEntity> TableNoTracking { get; }

    }
}
