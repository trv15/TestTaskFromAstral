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
    public class VacancyManager
    {
        private ApplicationContext _context;
        private ISharedStore<Vacancy, string> _store;

        public VacancyManager(ApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._store = new VacancyStore(context);
        }

        /*Свойство для пользовательских запросов*/
        public virtual IQueryable<Vacancy> QueryableVacancySet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_store == null)
                    throw new NotSupportedException();

                return _store.QueryableSet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        public virtual async Task<OperationResult> CreateAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.VacancyExistsIdAsync(vacancy.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Вакансия с таким ID уже присутствует в БД, свойство: ", nameof(vacancy.Id)));
            }
            else
            {
               await this._store.CreateAsync(vacancy);
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //обновляем вакансию
        public virtual async Task<OperationResult> UpdateAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.VacancyExistsIdAsync(vacancy.Id))
            {
                await this._store.UpdateAsync(vacancy);
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Вакансия с таким ID не найдена в БД, свойство: ", nameof(vacancy.Id)));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //удаляем вакансию
        public virtual async Task<OperationResult> DeleteAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));

            if (await this.VacancyExistsIdAsync(vacancy.Id))
            {
                await this._store.DeleteAsync(vacancy);
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемую вакансию, свойство: ", nameof(vacancy));
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /*Поиск по Id*/
        public virtual async Task<Vacancy> FindByIdAsync(string vacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(vacancyId))
                throw new ArgumentNullException(nameof(vacancyId));

            return await this._store.FindByIdAsync(vacancyId);
        }

        /*Поиск вакансии по названию*/
        public virtual async Task<Vacancy> FindByNameAsync(string vacancyName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(vacancyName))
                throw new ArgumentNullException(nameof(vacancyName));

            return await this._store.FirstByNameAsync(vacancyName);
        }

        /*Получаем все вакансии*/
        public async Task<IEnumerable<Vacancy>> GetAllVacancyAsync()
        {
            this.ThrowIfDisposed();

            return await this._store.QueryableSet.ToListAsync();
        }

        /*Существует ли вакансия с таким названием*/
        public virtual async Task<bool> VacancyExistsNameAsync(string vacancyName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(vacancyName))
                throw new ArgumentNullException(nameof(vacancyName));

            return await this.FindByNameAsync(vacancyName) != null;
        }

        /*Существует ли вакансия по Id*/
        public virtual async Task<bool> VacancyExistsIdAsync(string vacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(vacancyId))
                throw new ArgumentNullException(nameof(vacancyId));

            Vacancy tempVacancy = await this.FindByIdAsync(vacancyId);

            if(tempVacancy != null)
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

                    this._store.Dispose();
                    this._store = null;

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
