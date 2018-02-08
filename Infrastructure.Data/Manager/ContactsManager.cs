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
    public class ContactsManager
    {
        private ApplicationContext _context;
        private ISharedStore<Contacts, string> _contactsStore;

        public ContactsManager(ApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            this._context = context;
            this._contactsStore = new ContactsStore(context);
        }

        /*Свойство для пользовательских запросов*/
        public virtual IQueryable<Contacts> QueryableContactsSet
        {
            get
            {
                this.ThrowIfDisposed();
                if (_contactsStore == null)
                    throw new NotSupportedException();

                return _contactsStore.QueryableSet;
            }
        }

        //---------------------------Методы CRUD-------------------------------------
        public virtual async Task<OperationResult> CreateAsync(Contacts contacts)
        {
            this.ThrowIfDisposed();
            if (contacts == null)
                throw new ArgumentNullException(nameof(contacts));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.ContactsExistsIdAsync(contacts.Id))
            {
                errorList.Add(new ValidationException("Ошибка: Запись с таким  ID уже присутствует в БД, свойство: ", nameof(contacts.Id)));
            }
            else
            {
               await this._contactsStore.CreateAsync(contacts);
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //обновляем тел
        public virtual async Task<OperationResult> UpdateAsync(Contacts contacts)
        {
            this.ThrowIfDisposed();
            if (contacts == null)
                throw new ArgumentNullException(nameof(contacts));

            List<ValidationException> errorList = new List<ValidationException>();

            if (await this.ContactsExistsIdAsync(contacts.Id))
            {
                await this._contactsStore.UpdateAsync(contacts);
            }
            else
            {
                errorList.Add(new ValidationException("Ошибка: Запись с контактами с таким ID не найдена в БД, свойство: ", nameof(contacts.Id)));
            }

            return errorList.Count <= 0 ? OperationResult.GetSuccess : OperationResult.GetFailed(errorList);
        }
        //удаляем
        public virtual async Task<OperationResult> DeleteAsync(Contacts contacts)
        {
            this.ThrowIfDisposed();
            if (contacts == null)
                throw new ArgumentNullException(nameof(contacts));

            if (await this.ContactsExistsIdAsync(contacts.Id))
            {
                await this._contactsStore.DeleteAsync(contacts);
                return OperationResult.GetSuccess;
            }

            ValidationException exception = new ValidationException("Ошибка: Не удалость найти в БД удаляемую запись контактов, свойство: ", nameof(contacts));
            List<ValidationException> error = new List<ValidationException>();
            error.Add(exception);
            return OperationResult.GetFailed(error);
        }

        /*Поиск по Id*/
        public virtual async Task<Contacts> FindByIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));

            return await this._contactsStore.FindByIdAsync(Id);
        }

        /*Получаем все контакт*/
        public async Task<IEnumerable<Contacts>> GetAllContactsAsync()
        {
            this.ThrowIfDisposed();

            return await this._contactsStore.QueryableSet.ToListAsync();
        }

        /*Существует ли контакт по Id*/
        public virtual async Task<bool> ContactsExistsIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));

            Contacts tempContacts = await this.FindByIdAsync(Id);

            if(tempContacts != null)
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

                    this._contactsStore.Dispose();
                    this._contactsStore = null;

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
