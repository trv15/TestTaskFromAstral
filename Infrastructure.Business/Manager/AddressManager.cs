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
    public class AddressManager : ISharedManager<Address, string, OperationResult>
    {
        private IApplicationContext _context;

        private IUnitOfWork _work;

        public AddressManager(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this._context = context;
            this._work = new UnitOfWork(_context);
        }

        /// <summary>
        /// Свойство для пользовательских запросов
        /// </summary>
        public virtual IQueryable<Address> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_work == null)
                    throw new NotSupportedException();

                return _work.AddressRepository.QueryableEntitySet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        /// <summary>
        /// Метод для записи вновь созданного адреса(если данные существуют произойдет обновление данных)
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.ExistsIdAsync(address.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Адрес с таким Id уже присутствует в БД, " +
                    "произошло обновление полей, свойство: ", address.Id));
                await UpdateAsync(address);
            }
            else
            {
                await Task.Factory.StartNew(() => { this._work.AddressRepository.Create(address); });
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для записи вновь создданных адресов(если данные существуют произойдет обновление данных)
        /// </summary>
        /// <param name="addressList"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateRangeAsync(IEnumerable<Address> addressList)
        {
            this.ThrowIfDisposed();
            if (addressList == null)
                throw new ArgumentNullException(nameof(addressList));

            List<ValidationException> errorList = new List<ValidationException>();

            foreach (Address address in addressList)
            {
                if (await this.ExistsIdAsync(address.Id))
                {
                    errorList.Add(new ValidationException("Ошибка: Адрес с таким Id уже присутствует в БД, " +
                        "произошло обновление полей, свойство: ", address.Id));
                    await UpdateAsync(address);
                }
                else
                {
                    await Task.Factory.StartNew(() => { this._work.AddressRepository.Create(address); });
                }
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для сохранения измененных данных адреса
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> UpdateAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            List<ValidationException> errorList = new List<ValidationException>();

            Address updateAddr = await _work.AddressRepository.FindByIdAsync(address.Id);

            if (updateAddr != null)
            {
                updateAddr.City = address.City;
                updateAddr.Street = address.Street;
                updateAddr.Building = address.Building;
                updateAddr.Description = address.Description;

                await Task.Factory.StartNew(() => {
                    this._work.AddressRepository.Update(updateAddr);
                });
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Адреса с таким Id не найдено в БД: ", address.Id));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для удаления адреса
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> DeleteAsync(Address address)
        {
            this.ThrowIfDisposed();
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            if (await this.ExistsIdAsync(address.Id))
            {
                await Task.Factory.StartNew(() => {
                    this._work.AddressRepository.Delete(address);
                });
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемый адрес работодателя, Id: ", address.Id);
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /// <summary>
        /// Поиск адреса по Id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public virtual async Task<Address> FindByIdAsync(string addressId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(addressId))
                throw new ArgumentNullException(nameof(addressId));

            return await this._work.AddressRepository.FindByIdAsync(addressId);
        }

        /// <summary>
        /// Метод для получения всех адресов
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            this.ThrowIfDisposed();

            return await this._work.AddressRepository.QueryableEntitySet.ToListAsync();
        }

        /// <summary>
        /// Метод для проверки существует ли адрес по Id
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsIdAsync(string addressId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(addressId))
                throw new ArgumentNullException(nameof(addressId));

            Address address = await this.FindByIdAsync(addressId);

            if (address != null)
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
