using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Infrastructure.Data.Interfaces
{
    public interface IRepositoryGeneric<TEntity, TKey> where TEntity : class
    {
        IQueryable<TEntity> QueryableEntitySet { get; }

        DbContext Context { get; }

        DbSet<TEntity> DbEntitySet { get; }

        Task<TEntity> FindByIdAsync(TKey id);

        void Create(TEntity entity);

        void Delete(TEntity entity);

        void Update(TEntity entity);
    }
}
