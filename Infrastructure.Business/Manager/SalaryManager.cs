using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Context;
using Domain.Core;
using Infrastructure.Business.Validator;
using System.Data.Entity;
using System.Data;
using Infrastructure.Business.Interfaces;
using Infrastructure.Data.UnitOfWork;

namespace Infrastructure.Business.Manager
{
    public class SalaryManager : ISharedManager<Salary, string, OperationResult>
    {
        private IApplicationContext _context;

        private IUnitOfWork _work;

        public SalaryManager(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this._context = context;
            this._work = new UnitOfWork(_context);
        }

        /// <summary>
        /// Свойство для пользовательских запросов
        /// </summary>
        public virtual IQueryable<Salary> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_work == null)
                    throw new NotSupportedException();

                return _work.SalaryRepository.QueryableEntitySet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        /// <summary>
        /// Метод для записи вновь созданных данных в БД(если данные уже есть то произойдет обновление полей)
        /// </summary>
        /// <param name="salary"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.ExistsIdAsync(salary.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Запись о ЗП с таким Id уже присутствует в БД," +
                                                  " произошло обновление полей: ", salary.Id));
                await UpdateAsync(salary);
            }
            else
            {
                await Task.Factory.StartNew(() => { this._work.SalaryRepository.Create(salary); });
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для записи вновь созданных данных в БД(если данные уже есть то произойдет обновление полей)
        /// </summary>
        /// <param name="salaryList"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateRangeAsync(IEnumerable<Salary> salaryList)
        {
            this.ThrowIfDisposed();
            if (salaryList == null)
                throw new ArgumentNullException(nameof(salaryList));

            List<ValidationException> errorList = new List<ValidationException>();

            foreach (Salary salary in salaryList)
            {
                if (await this.ExistsIdAsync(salary.Id))
                {
                    errorList.Add(new ValidationException("Ошибка: Запись о ЗП с таким Id уже присутствует в БД," +
                        " произошло обновление полей: ", salary.Id));
                    await UpdateAsync(salary);
                }
                else
                {
                    await Task.Factory.StartNew(() => { this._work.SalaryRepository.Create(salary); });
                }
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для обновления данных зп в БД
        /// </summary>
        /// <param name="salary"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> UpdateAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));

            List<ValidationException> errorList = new List<ValidationException>();

            Salary updateSal = await _work.SalaryRepository.FindByIdAsync(salary.Id);

            if (updateSal != null)
            {
                updateSal.From = salary.From;
                updateSal.To = salary.To;
                updateSal.Gross = salary.Gross;
                updateSal.Currency = salary.Currency;

                await Task.Factory.StartNew(() => {
                    this._work.SalaryRepository.Update(updateSal);
                });
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Запись о зп с таким Id не найдена в БД: ", salary.Id));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для удаления зп из БД
        /// </summary>
        /// <param name="salary"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> DeleteAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));

            if (await this.ExistsIdAsync(salary.Id))
            {
                await Task.Factory.StartNew(() => {
                    this._work.SalaryRepository.Delete(salary);
                });
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемую запись о ЗП, Id: ", salary.Id);
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /// <summary>
        /// Поиск зп по Id
        /// </summary>
        /// <param name="salaryId"></param>
        /// <returns></returns>
        public virtual async Task<Salary> FindByIdAsync(string salaryId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(salaryId))
                throw new ArgumentNullException(nameof(salaryId));

            return await this._work.SalaryRepository.FindByIdAsync(salaryId);
        }

        /// <summary>
        /// Получаем все зп
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Salary>> GetAllAsync()
        {
            this.ThrowIfDisposed();

            return await this._work.SalaryRepository.QueryableEntitySet.ToListAsync();
        }

        /// <summary>
        /// Существует ли зп у вакансии по Id
        /// </summary>
        /// <param name="salaryId"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsIdAsync(string salaryId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(salaryId))
                throw new ArgumentNullException(nameof(salaryId));

            Salary salary = await this.FindByIdAsync(salaryId);

            if (salary != null)
                return true;

            return false;
        }

        /// <summary>
        /// Реализация интерфейса IDisposable
        /// </summary>
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

                    this._work.Dispose();
                    this._work = null;

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
