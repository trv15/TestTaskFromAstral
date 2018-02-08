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
    public class TypeVakancyManager
    {
        private ApplicationContext _context;
        private ISharedStore<TypeVakancy, string> _typeVakancyStore;

        public TypeVakancyManager(ApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._typeVakancyStore = new TypeVakancyStore(context);
        }

        /*Свойство для пользовательских запросов*/
        public virtual IQueryable<TypeVakancy> QueryableTypeVakancySet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_typeVakancyStore == null)
                    throw new NotSupportedException();

                return _typeVakancyStore.QueryableSet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        public virtual async Task<OperationResult> CreateAsync(TypeVakancy typeVakancy)
        {
            this.ThrowIfDisposed();
            if (typeVakancy == null)
                throw new ArgumentNullException(nameof(typeVakancy));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.TypeVakancyExistsIdAsync(typeVakancy.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Тип вакансии с таким ID уже присутствует в БД, свойство: ", nameof(typeVakancy.Id)));
            }
            else
            {
               await this._typeVakancyStore.CreateAsync(typeVakancy);
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //обновляем тип вакансии
        public virtual async Task<OperationResult> UpdateAsync(TypeVakancy typeVakancy)
        {
            this.ThrowIfDisposed();
            if (typeVakancy == null)
                throw new ArgumentNullException(nameof(typeVakancy));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.TypeVakancyExistsIdAsync(typeVakancy.Id))
            {
                await this._typeVakancyStore.UpdateAsync(typeVakancy);
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Тип вакансии с таким ID не найдена в БД, свойство: ", nameof(typeVakancy.Id)));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //удаляем тип вакансии
        public virtual async Task<OperationResult> DeleteAsync(TypeVakancy typeVakancy)
        {
            this.ThrowIfDisposed();
            if (typeVakancy == null)
                throw new ArgumentNullException(nameof(typeVakancy));

            if (await this.TypeVakancyExistsIdAsync(typeVakancy.Id))
            {
                await this._typeVakancyStore.DeleteAsync(typeVakancy);
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемый тип вакансии, свойство: ", nameof(typeVakancy));
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /*Поиск по Id*/
        public virtual async Task<TypeVakancy> FindByIdAsync(string typeVakancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(typeVakancyId))
                throw new ArgumentNullException(nameof(typeVakancyId));

            return await this._typeVakancyStore.FindByIdAsync(typeVakancyId);
        }

        /*Поиск типа вакансии по названию*/
        public virtual async Task<TypeVakancy> FindByNameAsync(string typeVakancyName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(typeVakancyName))
                throw new ArgumentNullException(nameof(typeVakancyName));

            return await this._typeVakancyStore.FirstByNameAsync(typeVakancyName);
        }

        /*Получаем все типы вакансии*/
        public async Task<IEnumerable<TypeVakancy>> GetAllTypeVakancyAsync()
        {
            this.ThrowIfDisposed();

            return await this._typeVakancyStore.QueryableSet.ToListAsync();
        }

        /*Существует ли тип вакансии с таким названием*/
        public virtual async Task<bool> TypeVakancyExistsNameAsync(string typeVakancyName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(typeVakancyName))
                throw new ArgumentNullException(nameof(typeVakancyName));

            return await this.FindByNameAsync(typeVakancyName) != null;
        }

        /*Существует ли тип вакансии по Id*/
        public virtual async Task<bool> TypeVakancyExistsIdAsync(string typeVacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(typeVacancyId))
                throw new ArgumentNullException(nameof(typeVacancyId));

            TypeVakancy tempTypeVakancy = await this.FindByIdAsync(typeVacancyId);

            if(tempTypeVakancy != null)
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

                    this._typeVakancyStore.Dispose();
                    this._typeVakancyStore = null;

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
