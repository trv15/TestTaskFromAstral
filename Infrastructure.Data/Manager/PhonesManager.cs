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
    public class PhonesManager
    {
        private ApplicationContext _context;
        private ISharedStore<Phones, Guid> _phonesStore;

        public PhonesManager(ApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._phonesStore = new PhonesStore(context);
        }

        /*Свойство для пользовательских запросов*/
        public virtual IQueryable<Phones> QueryablePhonesSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_phonesStore == null)
                    throw new NotSupportedException();

                return _phonesStore.QueryableSet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        public virtual async Task<OperationResult> CreateAsync(Phones phones)
        {
            this.ThrowIfDisposed();
            if (phones == null)
                throw new ArgumentNullException(nameof(phones));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.PhonesExistsIdAsync(phones.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Запись с таким  ID уже присутствует в БД, свойство: ", nameof(phones.Id)));
            }
            else
            {
               await this._phonesStore.CreateAsync(phones);
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //обновляем тел
        public virtual async Task<OperationResult> UpdateAsync(Phones phones)
        {
            this.ThrowIfDisposed();
            if (phones == null)
                throw new ArgumentNullException(nameof(phones));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.PhonesExistsIdAsync(phones.Id))
            {
                await this._phonesStore.UpdateAsync(phones);
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Запись с телефонным номером с таким ID не найдена в БД, свойство: ", nameof(phones.Id)));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //удаляем тел
        public virtual async Task<OperationResult> DeleteAsync(Phones phones)
        {
            this.ThrowIfDisposed();
            if (phones == null)
                throw new ArgumentNullException(nameof(phones));

            if (await this.PhonesExistsIdAsync(phones.Id))
            {
                await this._phonesStore.DeleteAsync(phones);
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемую запись с номером телефона, свойство: ", nameof(phones));
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /*Поиск по Id*/
        public virtual async Task<Phones> FindByIdAsync(Guid phonesId)
        {
            this.ThrowIfDisposed();
            if (Guid.Empty == phonesId || phonesId == null)
                throw new ArgumentNullException(nameof(phonesId));

            return await this._phonesStore.FindByIdAsync(phonesId);
        }

        /*Получаем все тел*/
        public async Task<IEnumerable<Phones>> GetAllPhonesAsync()
        {
            this.ThrowIfDisposed();

            return await this._phonesStore.QueryableSet.ToListAsync();
        }

        /*Существует ли тел по Id*/
        public virtual async Task<bool> PhonesExistsIdAsync(Guid phonesId)
        {
            this.ThrowIfDisposed();
            if (Guid.Empty == phonesId || phonesId == null)
                throw new ArgumentNullException(nameof(phonesId));

            Phones tempPhones = await this.FindByIdAsync(phonesId);

            if(tempPhones != null)
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

                    this._phonesStore.Dispose();
                    this._phonesStore = null;

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
