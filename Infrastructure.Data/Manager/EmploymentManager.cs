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
    public class EmploymentManager
    {
        private ApplicationContext _context;
        private ISharedStore<Employment, string> _employmentStore;

        public EmploymentManager(ApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._employmentStore = new EmploymentStore(context);
        }

        /*Свойство для пользовательских запросов*/
        public virtual IQueryable<Employment> QueryableEmploymentSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_employmentStore == null)
                    throw new NotSupportedException();

                return _employmentStore.QueryableSet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        public virtual async Task<OperationResult> CreateAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));

            List<ValidationException> errorList = new List<ValidationException>();

            bool res = await this.EmploymentExistsIdAsync(employment.Id);

            if (res)
            {
                errorList.Add(new ValidationException("Ошибка: Тип занятости с таким ID уже присутствует в БД, свойство: ", nameof(employment.Id)));
            }
            else
            {
               await this._employmentStore.CreateAsync(employment);
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //обновляем тип занят
        public virtual async Task<OperationResult> UpdateAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.EmploymentExistsIdAsync(employment.Id))
            {
                await this._employmentStore.UpdateAsync(employment);
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Тип занятости с таким ID не найдена в БД, свойство: ", nameof(employment.Id)));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //удаляем тип зан
        public virtual async Task<OperationResult> DeleteAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));

            if (await this.EmploymentExistsIdAsync(employment.Id))
            {
                await this._employmentStore.DeleteAsync(employment);
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемый тип занятости, свойство: ", nameof(employment));
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /*Поиск по Id*/
        public virtual async Task<Employment> FindByIdAsync(string employmentId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(employmentId))
                throw new ArgumentNullException(nameof(employmentId));

            return await this._employmentStore.FindByIdAsync(employmentId);
        }

        /*Поиск по названию*/
        public virtual async Task<Employment> FindByNameAsync(string employmentName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(employmentName))
                throw new ArgumentNullException(nameof(employmentName));

            return await this._employmentStore.FirstByNameAsync(employmentName);
        }

        /*Получаем все типы занятости*/
        public async Task<IEnumerable<Employment>> GetAllEmploymentAsync()
        {
            this.ThrowIfDisposed();

            return await this._employmentStore.QueryableSet.ToListAsync();
        }

        /*Существует ли тип занятости с таким названием*/
        public virtual async Task<bool> EmploymentExistsNameAsync(string employmentName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(employmentName))
                throw new ArgumentNullException(nameof(employmentName));

            return await this.FindByNameAsync(employmentName) != null;
        }

        /*Существует ли тип занятости по Id*/
        public virtual async Task<bool> EmploymentExistsIdAsync(string employmentId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(employmentId))
                throw new ArgumentNullException(nameof(employmentId));

            Employment tempEmployment = await this.FindByIdAsync(employmentId);

            if(tempEmployment != null)
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

                    this._employmentStore.Dispose();
                    this._employmentStore = null;

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
