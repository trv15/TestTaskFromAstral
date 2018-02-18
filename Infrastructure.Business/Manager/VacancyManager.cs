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
    public class VacancyManager : ISharedManager<Vacancy, string, OperationResult>
    {
        private IApplicationContext _context;

        private IUnitOfWork _work;

        public VacancyManager(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this._context = context;
            this._work = new UnitOfWork(_context);
        }

        /// <summary>
        ///Свойство для пользовательских запросов
        /// </summary>
        public virtual IQueryable<Vacancy> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_work == null)
                    throw new NotSupportedException();

                return _work.VacancyRepository.QueryableEntitySet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        /// <summary>
        /// Метод для записи вновь созданной вакансии(если данные уже существуют то они обновляются)
        /// </summary>
        /// <param name="vacancy"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.ExistsIdAsync(vacancy.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Вакансия с таким Id уже присутствует в БД, произошло обновление полей вакансии: ", vacancy.Id));

                await UpdateAsync(vacancy);
            }
            else
            {
                await Task.Factory.StartNew(() => { this._work.VacancyRepository.Create(vacancy); });
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для записи в бд вновь созданной коллекции вакансии(если данные уже существуют то они обновляются)
        /// </summary>
        /// <param name="vacancyList"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateRangeAsync(IEnumerable<Vacancy> vacancyList)
        {
            this.ThrowIfDisposed();
            if (vacancyList == null)
                throw new ArgumentNullException(nameof(vacancyList));

            List<ValidationException> errorList = new List<ValidationException>();

            foreach(Vacancy vacancy in vacancyList)
            {
                if (await this.ExistsIdAsync(vacancy.Id))
                {
                    errorList.Add(new ValidationException("Ошибка: Вакансия с таким Id уже присутствует в БД, произошло обновление полей вакансии: ", vacancy.Id));

                    await UpdateAsync(vacancy);
                }
                else
                {
                    await Task.Factory.StartNew(() => { this._work.VacancyRepository.Create(vacancy); });
                }
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для сохранения в бд изменении вакансии
        /// </summary>
        /// <param name="vacancy"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> UpdateAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));

            List<ValidationException> errorList = new List<ValidationException>();

            Vacancy updateVac = await _work.VacancyRepository.FindByIdAsync(vacancy.Id);

            if (updateVac != null)
            {
                updateVac.Name = vacancy.Name;
                updateVac.Published_At = vacancy.Published_At;
                updateVac.Description = vacancy.Description;
                updateVac.Archived = vacancy.Archived;
                updateVac.TypeVakancyId = vacancy.TypeVakancyId;
                updateVac.EmploymentId = vacancy.EmploymentId;

                await Task.Factory.StartNew(()=>{ this._work.VacancyRepository.Update(updateVac); });
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Вакансия с таким Id не найдена в БД: ", vacancy.Id));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для удаления вакансии
        /// </summary>
        /// <param name="vacancy"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> DeleteAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));

            if (await this.ExistsIdAsync(vacancy.Id))
            {
                await Task.Factory.StartNew(()=> {
                    this._work.VacancyRepository.Delete(vacancy);
                });
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемую вакансию, Id: ", vacancy.Id);
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /// <summary>
        /// Метод для поиска вакансии по Id
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        public virtual async Task<Vacancy> FindByIdAsync(string vacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(vacancyId))
                throw new ArgumentNullException(nameof(vacancyId));

            return await this._work.VacancyRepository.FindByIdAsync(vacancyId);
        }

        /// <summary>
        ///Метод для поиска вакансии по названию
        /// </summary>
        /// <param name="vacancyName"></param>
        /// <returns></returns>
        public virtual async Task<Vacancy> FindByNameAsync(string vacancyName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(vacancyName))
                throw new ArgumentNullException(nameof(vacancyName));

            return await this._work.VacancyRepository.QueryableEntitySet.FirstOrDefaultAsync(x=>x.Name.ToUpper() == vacancyName.ToUpper());
        }

        /// <summary>
        /// Метод для полуения всех вакансии
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Vacancy>> GetAllAsync()
        {
            this.ThrowIfDisposed();

            return await this._work.VacancyRepository.QueryableEntitySet.ToListAsync();
        }

        /// <summary>
        /// Метод для проверки существует ли вакансия с таким названием
        /// </summary>
        /// <param name="vacancyName"></param>
        /// <returns></returns>
        private async Task<bool> VacancyExistsNameAsync(string vacancyName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(vacancyName))
                throw new ArgumentNullException(nameof(vacancyName));

            return await this.FindByNameAsync(vacancyName) != null;
        }

        /// <summary>
        /// Метод для проверки существует ли вакансия по Id
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsIdAsync(string vacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(vacancyId))
                throw new ArgumentNullException(nameof(vacancyId));

            Vacancy tempVacancy = await this.FindByIdAsync(vacancyId);

            if(tempVacancy != null)
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
