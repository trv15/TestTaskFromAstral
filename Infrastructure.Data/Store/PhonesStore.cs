using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Core;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Repositories;
using Infrastructure.Data.Context;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Infrastructure.Data.Store
{
    public class PhonesStore : ISharedStore<Phones, Guid>
    {
        private IRepositoryGeneric<Phones, Guid> _repositoryPhones;
        public ApplicationContext _context { get; private set; }

        /*Конструктор*/
        public PhonesStore(ApplicationContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._repositoryPhones = new RepositoryGeneric<Phones, Guid>(context);
        }
        /*Свойство для пользовательских запросов*/
        public IQueryable<Phones> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                return this._repositoryPhones.QueryableEntitySet;
            }
        }
        /*Поиск тел по Id*/
        public Task<Phones> FindByIdAsync(Guid PhonesId)
        {
            this.ThrowIfDisposed();
            if (Guid.Empty == PhonesId)
                throw new ArgumentNullException(nameof(PhonesId));
            return this._repositoryPhones.FindByIdAsync(PhonesId);
        }
        /*Создаем новую запись тел*/
        public virtual async Task CreateAsync(Phones phones)
        {
            this.ThrowIfDisposed();
            if (phones == null)
                throw new ArgumentNullException(nameof(phones));
            this._repositoryPhones.Create(phones);

            await this._context.SaveChangesAsync();
        }
        /*Удаляем  запись тел*/
        public virtual async Task DeleteAsync(Phones phones)
        {
            this.ThrowIfDisposed();
            if (phones == null)
                throw new ArgumentNullException(nameof(phones));
            this._repositoryPhones.Delete(phones);

            await this._context.SaveChangesAsync();
        }
        /*Обновляем изменненную запись тел*/
        public virtual async Task UpdateAsync(Phones phones)
        {
            this.ThrowIfDisposed();
            if (phones == null)
                throw new ArgumentNullException(nameof(phones));
            this._repositoryPhones.Update(phones);

            await this._context.SaveChangesAsync();
        }

        /*Реализация интерфейса IDisposable.*/
        public void Dispose()
        {
            if (_disposed)
                return;
            this.Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }
        /*Вызывался ли Dispose для this*/
        private bool _disposed = false;
        /*Метод для освобождения управляемых ресурсов*/
        protected virtual void Dispose(bool disposing)
        {
            /*Проверяем если вызывали Dispose контекста то просто обнуляем ссылку на контекст, 
             если Dispose контекста не вызывался то вызыввем и обнуляем ссылки*/
            if (!this._disposed)
            {
                if (this._context.GetBoolDisposeContext == false && disposing)
                {
                    this._context.Dispose();
                }
                this._context = null;
                this._repositoryPhones = null;
                this._disposed = true;
            }
        }
        /*Если был вызван Dispose то при попытке использования методов бросаем исключение*/
        private void ThrowIfDisposed()
        {
            if (this._disposed || _context.GetBoolDisposeContext)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        public Task<Phones> FirstByNameAsync(string entityName)
        {
            throw new NotImplementedException();
        }
    }
}
