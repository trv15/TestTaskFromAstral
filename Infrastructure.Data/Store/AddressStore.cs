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
    public class AddressStore : ISharedStore<Address, string>
    {
        private IRepositoryGeneric<Address, string> _repositoryAddress;
        public ApplicationContext _context { get; private set; }

        /*Конструктор*/
        public AddressStore(ApplicationContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._repositoryAddress = new RepositoryGeneric<Address, string>(context);
        }
        /*Свойство для пользовательских запросов*/
        public IQueryable<Address> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                return this._repositoryAddress.QueryableEntitySet;
            }
        }
        /*Поиск адреса по Id*/
        public Task<Address> FindByIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));
            return this._repositoryAddress.FindByIdAsync(Id);
        }
        /*Создаем новую запись адреса*/
        public virtual async Task CreateAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            this._repositoryAddress.Create(address);

            await this._context.SaveChangesAsync();
        }
        /*Удаляем  запись адреса*/
        public virtual async Task DeleteAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            this._repositoryAddress.Delete(address);

            await this._context.SaveChangesAsync();
        }
        /*Обновляем изменненную запись*/
        public virtual async Task UpdateAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            this._repositoryAddress.Update(address);

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
                this._repositoryAddress = null;
                this._disposed = true;
            }
        }
        /*Если был вызван Dispose то при попытке использования методов бросаем исключение*/
        private void ThrowIfDisposed()
        {
            if (this._disposed || _context.GetBoolDisposeContext)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        public Task<Address> FirstByNameAsync(string entityName)
        {
            throw new NotImplementedException();
        }
    }
}
