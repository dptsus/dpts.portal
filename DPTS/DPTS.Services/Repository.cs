using DPTS.Data.Context;
using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using System.Reflection;


namespace DPTS.Services
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        #region Fields
        protected readonly DPTSDbContext _context;
        private DbSet<TEntity> _entities;
        #endregion

        #region Constructor
        public Repository(DPTSDbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        #endregion

        #region Utility Methods
        private void SetDateTimePropertiesIfAny(TEntity entity, bool isAddOperation = true)
        {
            var typeInfo = entity.GetType();
            if (isAddOperation)
            {
                var dateCreatedProperty = typeInfo.GetProperty(nameof(BaseEntityWithDateTime.DateCreated));
                if (dateCreatedProperty != null)
                {
                    (entity as BaseEntityWithDateTime).DateCreated = DateTime.UtcNow;
                }
            }

            var dateUpdatedProperty = typeInfo.GetProperty(nameof(BaseEntityWithDateTime.DateUpdated));
            if (dateUpdatedProperty != null)
            {
                (entity as BaseEntityWithDateTime).DateUpdated = DateTime.UtcNow;
            }

        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table
        {
            get
            {
                return _entities;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<TEntity> TableNoTracking
        {
            get
            {
                return _entities.AsNoTracking();
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Add new entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Task</returns>
        public virtual void AddAsync(TEntity entity)
        {
            SetDateTimePropertiesIfAny(entity);

             _entities.Add(entity);

             _context.SaveChangesAsync();

        }

        /// <summary>
        /// Update existing entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Task</returns>
        public virtual void UpdateAsync(TEntity entity)
        {
            SetDateTimePropertiesIfAny(entity, isAddOperation: false);

          //  _entities.update(entity);

            _context.SaveChangesAsync();

        }

        /// <summary>
        /// Remove entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public async Task RemoveAsync(TEntity entity)
        {
            _entities.Remove(entity);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Task<Entity></returns>
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <param name="enableTracking">Enable Entity Tracking. Default is true</param>
        /// <returns>Task<IEnumerable<TEntity>></returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool enableTracking = false)
        {
            return enableTracking ? await _entities.ToListAsync()
                : await _entities.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Find entities matching the specified condition
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Task<IEnumerable<TEntity>></returns>
        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _entities.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Find only single record matching the specified condition
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Task<TEntity></returns>
        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Return true if records for the specified condition exists
        /// </summary>
        /// <param name="predicate">Condition</param>
        /// <returns>Task<Bool></returns>
        public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.AnyAsync(predicate);
        }
        #endregion
    }
}
