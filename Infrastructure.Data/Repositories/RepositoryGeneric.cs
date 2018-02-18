using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Context;


namespace Infrastructure.Data.Repositories
{
    internal class RepositoryGeneric<TEntity, TKey> : IRepositoryGeneric<TEntity, TKey> where TEntity : class
    {
        private ApplicationContext _context;
        private DbSet<TEntity> _dbEntitySet;

        public RepositoryGeneric(ApplicationContext context)
        {
            this._context = context ?? throw new ArgumentNullException(context.GetType().Name);
            this._dbEntitySet = _context.Set<TEntity>();
        }

        public IApplicationContext Context
        {
            get
            {
                return this._context;
            }
        }

        public DbSet<TEntity> DbEntitySet
        {
            get
            {
                return _dbEntitySet;
            }
        }

        public IQueryable<TEntity> QueryableEntitySet
        {
            get
            {
                return (IQueryable<TEntity>)this._dbEntitySet;
            }
        }

        public virtual Task<TEntity> FindByIdAsync(TKey id)
        {
            return this._dbEntitySet.FindAsync(id);
        }

        public virtual void Create(TEntity entity)
        {
            this._dbEntitySet.Add(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            this._dbEntitySet.Remove(entity);
        }

        public virtual void Update(TEntity entity)
        {
            this._context.Entry<TEntity>(entity).State = EntityState.Modified;
        }

        public void CreateRange(IEnumerable<TEntity> entity)
        {
            this._dbEntitySet.AddRange(entity);
        }
    }
}
