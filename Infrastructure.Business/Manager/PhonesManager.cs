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
    public class PhonesManager : ISharedManager<Phones, Guid, OperationResult>
    {
        private IApplicationContext _context;

        private IUnitOfWork _work;

        public PhonesManager(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this._context = context;
            this._work = new UnitOfWork(_context);
        }

        /// <summary>
        /// Свойство для пользовательских запросов
        /// </summary>
        public virtual IQueryable<Phones> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_work == null)
                    throw new NotSupportedException();

                return _work.PhonesRepository.QueryableEntitySet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        /// <summary>
        /// Метод для записи в БД вновь измененных (если данные сущ то произойдет авто обновление)
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateAsync(Phones phone)
        {
            this.ThrowIfDisposed();
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await IsInPhoneAsync(phone))
            {
                errorList.Add(new ValidationException("Ошибка: Телефонный номер присутствует в БД: ", ""));
                await UpdateAsync(phone);
            }
            else
            {
                phone.Id = Guid.NewGuid();

                await Task.Factory.StartNew(() => { this._work.PhonesRepository.Create(phone); });
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для проверки соотвествия тел номера
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        private async Task<bool> IsInPhoneAsync(Phones phone)
        {
            return await _work.PhonesRepository.DbEntitySet.AnyAsync(x=>x.City == phone.City & x.Country == phone.Country);
        }
        /// <summary>
        /// Метод для записи в БД вновь измененных (если данные сущ то произойдет авто обновление)
        /// </summary>
        /// <param name="phonesList"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateRangeAsync(IEnumerable<Phones> phonesList)
        {
            this.ThrowIfDisposed();
            if (phonesList == null)
                throw new ArgumentNullException(nameof(phonesList));

            List<ValidationException> errorList = new List<ValidationException>();

            foreach (Phones phone in phonesList)
            {
                if (await IsInPhoneAsync(phone))
                {
                    errorList.Add(new ValidationException("Ошибка: Телефонный номер присутствует в БД: ", ""));
                    await UpdateAsync(phone);
                }
                else
                {
                    phone.Id = Guid.NewGuid();
                    await Task.Factory.StartNew(() => { this._work.PhonesRepository.Create(phone); });
                }
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для сохранения измененных данных в БД
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> UpdateAsync(Phones phone)
        {
            this.ThrowIfDisposed();
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));

            List<ValidationException> errorList = new List<ValidationException>();

            Phones phoneUpd = await _work.PhonesRepository.FindByIdAsync(phone.Id);

            if (phoneUpd != null)
            {
                phoneUpd.Country = phoneUpd.Country;
                phoneUpd.City = phone.City;
                phoneUpd.Number = phone.Number;
                phoneUpd.Comment = phone.Comment;
                phoneUpd.ContactsId = phone.ContactsId;              

                await Task.Factory.StartNew(() => {
                    this._work.PhonesRepository.Update(phoneUpd);
                });
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: телефонный номер с таким Id не найден в БД: ", phone.Id.ToString()));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для удаления тел номера из БД
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> DeleteAsync(Phones phone)
        {
            this.ThrowIfDisposed();
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));

            if (await this.ExistsIdAsync(phone.Id))
            {
                await Task.Factory.StartNew(() => {
                    this._work.PhonesRepository.Delete(phone);
                });
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемый телефонный номер: ", phone.Id.ToString());
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /// <summary>
        /// Поиск по Id
        /// </summary>
        /// <param name="phonesId"></param>
        /// <returns></returns>
        public virtual async Task<Phones> FindByIdAsync(Guid phonesId)
        {
            this.ThrowIfDisposed();
            if (Guid.Empty == phonesId)
                throw new ArgumentNullException(nameof(phonesId));

            return await this._work.PhonesRepository.FindByIdAsync(phonesId);
        }

        /// <summary>
        /// Получаем все телефоны
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Phones>> GetAllAsync()
        {
            this.ThrowIfDisposed();

            return await this._work.PhonesRepository.QueryableEntitySet.ToListAsync();
        }

        /// <summary>
        /// Существует ли тел номер по Id
        /// </summary>
        /// <param name="phonesId"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsIdAsync(Guid phonesId)
        {
            this.ThrowIfDisposed();
            if (Guid.Empty == phonesId)
                throw new ArgumentNullException(nameof(phonesId));

            Phones phones = await this.FindByIdAsync(phonesId);

            if (phones != null)
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
