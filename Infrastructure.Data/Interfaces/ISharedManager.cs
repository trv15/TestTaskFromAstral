using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Interfaces
{
    public interface ISharedManager<TEntity, TKey, TResult> : IDisposable where TEntity : class where TResult : class
    {
        IQueryable<TEntity> QueryableSet { get; }

        Task<TResult> CreateAsync(TEntity entity);

        Task<TResult> UpdateAsync(TEntity entity);

        Task<TResult> DeleteAsync(TEntity entity);

        Task<bool> ExistsIdAsync(TKey key);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> FindByIdAsync(TKey Id);
    }
}
