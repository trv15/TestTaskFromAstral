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
    public class EmploymentManager : ISharedManager<Employment, string, OperationResult>
    {
        private IApplicationContext _context;

        private IUnitOfWork _work;

        public EmploymentManager(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this._context = context;
            this._work = new UnitOfWork(_context);
        }

        /// <summary>
        /// Свойство для пользовательских запросов
        /// </summary>
        public virtual IQueryable<Employment> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_work == null)
                    throw new NotSupportedException();

                return _work.EmploymentRepository.QueryableEntitySet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        /// <summary>
        /// Метод для запаси в БД вновь созданного вида вакансии(если анные уже есть в бд они будут обновлены)
        /// </summary>
        /// <param name="employment"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.ExistsIdAsync(employment.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Вид вакансии с таким Id уже присутствует в БД, " +
                    "произошло обновление полей: ", employment.Id));
                await UpdateAsync(employment);
            }
            else
            {
                await Task.Factory.StartNew(() => { this._work.EmploymentRepository.Create(employment); });
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для запаси в БД вновь созданных видов вакансии(если анные уже есть в бд они будут обновлены)
        /// </summary>
        /// <param name="employmentList"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateRangeAsync(IEnumerable<Employment> employmentList)
        {
            this.ThrowIfDisposed();
            if (employmentList == null)
                throw new ArgumentNullException(nameof(employmentList));

            List<ValidationException> errorList = new List<ValidationException>();

            foreach (Employment employment in employmentList)
            {
                if (await this.ExistsIdAsync(employment.Id))
                {
                    errorList.Add(new ValidationException("Ошибка: Вид вакансии с таким Id уже присутствует в БД, " +
                        "произошло обновление полей: ", employment.Id));

                    await UpdateAsync(employment);
                }
                else
                {
                    await Task.Factory.StartNew(() => { this._work.EmploymentRepository.Create(employment); });
                }
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для обновления измененных данных вида вакансии в БД
        /// </summary>
        /// <param name="employment"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> UpdateAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));

            List<ValidationException> errorList = new List<ValidationException>();

            Employment updateEmpl = await _work.EmploymentRepository.FindByIdAsync(employment.Id);

            if (updateEmpl != null)
            {
                updateEmpl.Name = employment.Name;

                await Task.Factory.StartNew(() => {
                    this._work.EmploymentRepository.Update(updateEmpl);
                });
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Вид вакансии с таким Id не найден в БД: ", employment.Id));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для удаления вида вакансии из БД
        /// </summary>
        /// <param name="employment"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> DeleteAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));

            if (await this.ExistsIdAsync(employment.Id))
            {
                await Task.Factory.StartNew(() => {
                    this._work.EmploymentRepository.Delete(employment);
                });
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемый вид вакансии, Id: ", employment.Id);
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /// <summary>
        /// Поиск вида вакансии по Id
        /// </summary>
        /// <param name="employmentId"></param>
        /// <returns></returns>
        public virtual async Task<Employment> FindByIdAsync(string employmentId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(employmentId))
                throw new ArgumentNullException(nameof(employmentId));

            return await this._work.EmploymentRepository.FindByIdAsync(employmentId);
        }

        /// <summary>
        /// Метод для получения всех видов вакансии
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Employment>> GetAllAsync()
        {
            this.ThrowIfDisposed();

            return await this._work.EmploymentRepository.QueryableEntitySet.ToListAsync();
        }

        /// <summary>
        /// Метод для проверки существует ли вид вакансии по Id
        /// </summary>
        /// <param name="employmentId"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsIdAsync(string employmentId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(employmentId))
                throw new ArgumentNullException(nameof(employmentId));

            Employment employment = await this.FindByIdAsync(employmentId);

            if (employment != null)
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
