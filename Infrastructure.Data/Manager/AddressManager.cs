using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.Manager;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Store;
using Infrastructure.Data.Context;
using Domain.Core;
using Infrastructure.Data.Validator;
using System.Data.Entity;

namespace Infrastructure.Data.Manager
{
    public class AddressManager
    {
        private ApplicationContext _context;
        private ISharedStore<Address, string> _addressStore;

        public AddressManager(ApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._addressStore = new AddressStore(context);
        }

        /*Свойство для пользовательских запросов*/
        public virtual IQueryable<Address> QueryableAddressSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_addressStore == null)
                    throw new NotSupportedException();

                return _addressStore.QueryableSet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        public virtual async Task<OperationResult> CreateAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.AddressExistsIdAsync(address.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Запись с таким  ID уже присутствует в БД, свойство: ", nameof(address.Id)));
            }
            else
            {
               await this._addressStore.CreateAsync(address);
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //обновляем
        public virtual async Task<OperationResult> UpdateAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.AddressExistsIdAsync(address.Id))
            {
                await this._addressStore.UpdateAsync(address);
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Запись с таким ID не найдена в БД, свойство: ", nameof(address.Id)));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //удаляем
        public virtual async Task<OperationResult> DeleteAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            if (await this.AddressExistsIdAsync(address.Id))
            {
                await this._addressStore.DeleteAsync(address);
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемую запись, свойство: ", nameof(address));
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /*Поиск по Id*/
        public virtual async Task<Address> FindByIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));

            return await this._addressStore.FindByIdAsync(Id);
        }

        /*Получаем все адреса*/
        public async Task<IEnumerable<Address>> GetAllAddressAsync()
        {
            this.ThrowIfDisposed();

            return await this._addressStore.QueryableSet.ToListAsync();
        }

        /*Существует ли адресс по Id*/
        public virtual async Task<bool> AddressExistsIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));

            Address tempAddress = await this.FindByIdAsync(Id);

            if(tempAddress != null)
               return true;

            return false;
        }

        /*Управление уничтожением объектов*/
        public void Dispose()
        {
            if (this._disposed)
                return;
            this.DisposeFromAll(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;

        protected virtual void DisposeFromAll(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this._context != null)
                        this._context = null;

                    this._addressStore.Dispose();
                    this._addressStore = null;

                    this._disposed = true;
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }
    }
}
