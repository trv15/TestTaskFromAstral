using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Core;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Repositories;
using Infrastructure.Data.Context;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Infrastructure.Data.Store
{
    public class ContactsStore : ISharedStore<Contacts, string>
    {
        private IRepositoryGeneric<Contacts, string> _repositoryContacts;
        public ApplicationContext _context { get; private set; }

        /*Конструктор*/
        public ContactsStore(ApplicationContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._repositoryContacts = new RepositoryGeneric<Contacts, string>(context);
        }
        /*Свойство для пользовательских запросов*/
        public IQueryable<Contacts> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                return this._repositoryContacts.QueryableEntitySet;
            }
        }
        /*Поиск контактов по Id*/
        public Task<Contacts> FindByIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if(String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));
            return this._repositoryContacts.FindByIdAsync(Id);
        }
        /*Создаем новую запись контактоных данных*/
        public virtual async Task CreateAsync(Contacts contacts)
        {
            this.ThrowIfDisposed();
            if (contacts == null)
                throw new ArgumentNullException(nameof(contacts));
            this._repositoryContacts.Create(contacts);

            await this._context.SaveChangesAsync();
        }
        /*Удаляем  запись контактов*/
        public virtual async Task DeleteAsync(Contacts contacts)
        {
            this.ThrowIfDisposed();
            if (contacts == null)
                throw new ArgumentNullException(nameof(contacts));
            this._repositoryContacts.Delete(contacts);

            await this._context.SaveChangesAsync();
        }
        /*Обновляем изменненную запись конт*/
        public virtual async Task UpdateAsync(Contacts contacts)
        {
            this.ThrowIfDisposed();
            if (contacts == null)
                throw new ArgumentNullException(nameof(contacts));
            this._repositoryContacts.Update(contacts);

            await this._context.SaveChangesAsync();
        }

        /*Реализация интерфейса IDisposable.*/
        public void Dispose()
        {
            if (_disposed)
                return;
            this.Dispose(true);
            // подавляем финализацию
            GC.SuppressFinalize(this);
        }
        /*Вызывался ли Dispose для this*/
        private bool _disposed = false;
        /*Метод для освобождения управляемых ресурсов*/
        protected virtual void Dispose(bool disposing)
        {
            /*Проверяем если вызывали Dispose контекста то просто обнуляем ссылку на контекст, 
             если Dispose контекста не вызывался то вызыввем и обнуляем ссылки*/
            if (!this._disposed)
            {
                if (this._context.GetBoolDisposeContext == false && disposing)
                {
                    this._context.Dispose();
                }
                this._context = null;
                this._repositoryContacts = null;
                this._disposed = true;
            }
        }
        /*Если был вызван Dispose то при попытке использования методов бросаем исключение*/
        private void ThrowIfDisposed()
        {
            if (this._disposed || _context.GetBoolDisposeContext)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        public Task<Contacts> FirstByNameAsync(string entityName)
        {
            throw new NotImplementedException();
        }
    }
}
