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
    public class ContactsManager : ISharedManager<Contacts, string, OperationResult>
    {
        private IApplicationContext _context;

        private IUnitOfWork _work;

        public ContactsManager(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            this._context = context;
            this._work = new UnitOfWork(_context);
        }

        /// <summary>
        /// Свойство для пользовательских запросов
        /// </summary>
        public virtual IQueryable<Contacts> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_work == null)
                    throw new NotSupportedException();

                return _work.ContactsRepository.QueryableEntitySet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        /// <summary>
        /// Метод записи вновь созданного контакта(если данные существуют произойдет обновление данных)
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateAsync(Contacts contact)
        {
            this.ThrowIfDisposed();
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.ExistsIdAsync(contact.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Контакты с таким Id уже присутствуют в БД, Произошло обновление полей: ", contact.Id));
                await UpdateAsync(contact);
            }
            else
            {
                await Task.Factory.StartNew(() => { this._work.ContactsRepository.Create(contact); });
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод записи вновь созданных контактов(если данные существуют произойдет обновление данных)
        /// </summary>
        /// <param name="contactsList"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> CreateRangeAsync(IEnumerable<Contacts> contactsList)
        {
            this.ThrowIfDisposed();
            if (contactsList == null)
                throw new ArgumentNullException(nameof(contactsList));

            List<ValidationException> errorList = new List<ValidationException>();

            foreach (Contacts contact in contactsList)
            {
                if (await this.ExistsIdAsync(contact.Id))
                {
                    errorList.Add(new ValidationException("Ошибка: Контакты с таким Id уже присутствуют в БД, Произошло обновление полей: ", contact.Id));

                    await UpdateAsync(contact);
                }
                else
                {
                    await Task.Factory.StartNew(() => { this._work.ContactsRepository.Create(contact); });
                }
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для обновления в бд изменении контакта
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> UpdateAsync(Contacts contact)
        {
            this.ThrowIfDisposed();
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            List<ValidationException> errorList = new List<ValidationException>();

            Contacts updateCont = await _work.ContactsRepository.FindByIdAsync(contact.Id);

            if (updateCont != null)
            {
                updateCont.Name = contact.Name;
                updateCont.Email = contact.Email;

                await Task.Factory.StartNew(() => {
                    this._work.ContactsRepository.Update(updateCont);
                });
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Контактов с таким Id не найдено в БД, свойство: ", contact.Id));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        /// <summary>
        /// Метод для удаления контакта
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public virtual async Task<OperationResult> DeleteAsync(Contacts contact)
        {
            this.ThrowIfDisposed();
            if (contact == null)
                throw new ArgumentNullException(nameof(contact));

            if (await this.ExistsIdAsync(contact.Id))
            {
                await Task.Factory.StartNew(() => {
                    this._work.ContactsRepository.Delete(contact);
                });
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемые контакты, Id: ", contact.Id);
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /// <summary>
        /// Метод для поиска контакта по Id
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public virtual async Task<Contacts> FindByIdAsync(string contactId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(contactId))
                throw new ArgumentNullException(nameof(contactId));

            return await this._work.ContactsRepository.FindByIdAsync(contactId);
        }

        /// <summary>
        /// Метод для получения всех контактов
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Contacts>> GetAllAsync()
        {
            this.ThrowIfDisposed();

            return await this._work.ContactsRepository.QueryableEntitySet.ToListAsync();
        }

        /// <summary>
        /// Метод для проверки существует ли контакт по Id
        /// </summary>
        /// <param name="contactId"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsIdAsync(string contactId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(contactId))
                throw new ArgumentNullException(nameof(contactId));

            Contacts contact = await this.FindByIdAsync(contactId);

            if (contact != null)
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
