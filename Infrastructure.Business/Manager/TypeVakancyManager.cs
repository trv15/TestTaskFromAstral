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
    public class TypeVakancyManager : ISharedManager<TypeVacancy, string, OperationResult>
    {
        private IApplicationContext _context;

        private IUnitOfWork _work;

        public TypeVakancyManager(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this._context = context;
            this._work = new UnitOfWork(_context);
        }

        /// <summary>
        /// Свойство для пользовательских запросов
        /// </summary>
        public virtual IQueryable<TypeVacancy> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_work == null)
                    throw new NotSupportedException();

                return _work.TypeVakancyRepository.QueryableEntitySet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        /// <summary>
        /// Метод для записи в бд вновь созданного типа вакансии(если данные уже существуют то они обновляются)
        /// </summary>
        /// <param name="typeVacancy"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateAsync(TypeVacancy typeVacancy)
        {
            this.ThrowIfDisposed();
            if (typeVacancy == null)
                throw new ArgumentNullException(nameof(typeVacancy));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.ExistsIdAsync(typeVacancy.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Тип вакансии с таким Id уже присутствует в БД, " +
                    "произошло обновление полей типа вакансии: ", typeVacancy.Id));
                await UpdateAsync(typeVacancy);
            }
            else
            {
                await Task.Factory.StartNew(() => { this._work.TypeVakancyRepository.Create(typeVacancy); });
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для записи в бд вновь созданной коллекции типа вакансии(если данные уже существуют то они обновляются)
        /// </summary>
        /// <param name="typeVacancyList"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateRangeAsync(IEnumerable<TypeVacancy> typeVacancyList)
        {
            this.ThrowIfDisposed();
            if (typeVacancyList == null)
                throw new ArgumentNullException(nameof(typeVacancyList));

            List<ValidationException> errorList = new List<ValidationException>();

            
            foreach (TypeVacancy typeVacancy in typeVacancyList)
            {
                if (await this.ExistsIdAsync(typeVacancy.Id))
                {
                    errorList.Add(new ValidationException("Ошибка: Тип вакансии с таким Id уже присутствует в БД, " +
                        "произошло обновление полей типа вакансии: ", typeVacancy.Id));
                    await UpdateAsync(typeVacancy);
                }
                else
                {
                    await Task.Factory.StartNew(() => { this._work.TypeVakancyRepository.Create(typeVacancy); });
                }
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для записи обновленных данных типа вакансии
        /// </summary>
        /// <param name="typeVacancy"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> UpdateAsync(TypeVacancy typeVacancy)
        {
            this.ThrowIfDisposed();
            if (typeVacancy == null)
                throw new ArgumentNullException(nameof(typeVacancy));

            List<ValidationException> errorList = new List<ValidationException>();

            TypeVacancy updateType = await _work.TypeVakancyRepository.FindByIdAsync(typeVacancy.Id);

            if (updateType != null)
            {
                updateType.Name = typeVacancy.Name;

                await Task.Factory.StartNew(() => {
                    this._work.TypeVakancyRepository.Update(updateType);
                });
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Тип вакансии с таким Id не найден в БД: ", typeVacancy.Id));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для удаления типа вакансии
        /// </summary>
        /// <param name="typeVacancy"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> DeleteAsync(TypeVacancy typeVacancy)
        {
            this.ThrowIfDisposed();
            if (typeVacancy == null)
                throw new ArgumentNullException(nameof(typeVacancy));

            if (await this.ExistsIdAsync(typeVacancy.Id))
            {
                await Task.Factory.StartNew(() => {
                    this._work.TypeVakancyRepository.Delete(typeVacancy);
                });
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемый тип вакансии, Id: ", typeVacancy.Id);
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /// <summary>
        /// Метод для поиска по id типа вакансии
        /// </summary>
        /// <param name="typeVacancyId"></param>
        /// <returns></returns>
        public virtual async Task<TypeVacancy> FindByIdAsync(string typeVacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(typeVacancyId))
                throw new ArgumentNullException(nameof(typeVacancyId));

            return await this._work.TypeVakancyRepository.FindByIdAsync(typeVacancyId);
        }

        /// <summary>
        /// Метод для получения всех типов вакансии
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TypeVacancy>> GetAllAsync()
        {
            this.ThrowIfDisposed();

            return await this._work.TypeVakancyRepository.QueryableEntitySet.ToListAsync();
        }

        /// <summary>
        /// Метод для проверки существует ли тип вакансии по Id
        /// </summary>
        /// <param name="typeVacancyId"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsIdAsync(string typeVacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(typeVacancyId))
                throw new ArgumentNullException(nameof(typeVacancyId));

            TypeVacancy typeVacancy = await this.FindByIdAsync(typeVacancyId);

            if (typeVacancy != null)
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
