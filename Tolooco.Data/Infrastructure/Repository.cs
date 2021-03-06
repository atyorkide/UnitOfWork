﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tolooco.Data.Infrastructure
{
    public abstract class Repository<TEntity> : IRepository<TEntity>, IDisposable
        where TEntity : class
    {
        private readonly DbContext db;

        private IDbSet<TEntity> DbSet
        {
            get
            {
                return db.Set<TEntity>();
            }
        }

        public Repository(DbContext dbContext)
        {
            this.db = dbContext;
        }

        public virtual void Delete(object id)
        {
            var entity = GetById(id);
            if (entity == null) throw new ArgumentException("entity");
            DbSet.Remove(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> where)
        {
            IEnumerable<TEntity> objects = DbSet.Where(where).AsEnumerable();
            foreach (TEntity obj in objects) DbSet.Remove(obj);
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> where)
        {
            return DbSet.Where(where).FirstOrDefault();
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return DbSet.AsEnumerable();
        }

        public virtual async Task<List<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where)
        {
            return await DbSet.Where(where).FirstOrDefaultAsync();
        }

        public virtual TEntity GetById(object Id)
        {
            return DbSet.Find(Id);
        }

        public virtual IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            return DbSet.Where(where).ToList();
        }

        public virtual async Task<List<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where)
        {
            return await DbSet.Where(where).ToListAsync();
        }

        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentException("entity");
            DbSet.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
        }

        #region Dispose

        protected bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                db.Dispose();
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
