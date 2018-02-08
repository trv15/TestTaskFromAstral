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
    public class SalaryManager
    {
        private ApplicationContext _context;
        private ISharedStore<Salary, string> _salaryStore;

        public SalaryManager(ApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._salaryStore = new SalaryStore(context);
        }

        /*Свойство для пользовательских запросов*/
        public virtual IQueryable<Salary> QueryableSalarySet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_salaryStore == null)
                    throw new NotSupportedException();

                return _salaryStore.QueryableSet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        public virtual async Task<OperationResult> CreateAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.SalaryExistsIdAsync(salary.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Запись с таким ID уже присутствует в БД, свойство: ", nameof(salary.Id)));
            }
            else
            {
               await this._salaryStore.CreateAsync(salary);
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //обновляем зп
        public virtual async Task<OperationResult> UpdateAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.SalaryExistsIdAsync(salary.Id))
            {
                await this._salaryStore.UpdateAsync(salary);
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Запись с таким ID не найдена в БД, свойство: ", nameof(salary.Id)));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //удаляем зп
        public virtual async Task<OperationResult> DeleteAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));

            if (await this.SalaryExistsIdAsync(salary.Id))
            {
                await this._salaryStore.DeleteAsync(salary);
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемую запись, свойство: ", nameof(salary));
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /*Поиск по Id*/
        public virtual async Task<Salary> FindByIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));

            return await this._salaryStore.FindByIdAsync(Id);
        }

        /*Получаем все зп*/
        public async Task<IEnumerable<Salary>> GetAllSalaryAsync()
        {
            this.ThrowIfDisposed();

            return await this._salaryStore.QueryableSet.ToListAsync();
        }

        /*Существует ли зп по Id*/
        public virtual async Task<bool> SalaryExistsIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));

            Salary tempSalary = await this.FindByIdAsync(Id);

            if(tempSalary != null)
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

                    this._salaryStore.Dispose();
                    this._salaryStore = null;

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
