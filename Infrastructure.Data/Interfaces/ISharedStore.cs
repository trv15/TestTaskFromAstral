using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Interfaces
{
    public interface ISharedStore<TEntity, TKey> : IDisposable where TEntity : class
    {
        Task CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);

        Task<TEntity> FindByIdAsync(TKey entityId);

        Task<TEntity> FirstByNameAsync(string entityName);

        IQueryable<TEntity> QueryableSet { get; }
    }
}
