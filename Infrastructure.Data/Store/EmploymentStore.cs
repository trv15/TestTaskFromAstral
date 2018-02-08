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
    public class EmploymentStore : ISharedStore<Employment, string>
    {
        private IRepositoryGeneric<Employment, string> _repositoryEmployment;
        public ApplicationContext _context { get; private set; }

        /*Конструктор*/
        public EmploymentStore(ApplicationContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._repositoryEmployment = new RepositoryGeneric<Employment, string>(context);
        }
        /*Свойство для пользовательских запросов*/
        public IQueryable<Employment> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                return this._repositoryEmployment.QueryableEntitySet;
            }
        }
        /*Поиск типа занятости по Id*/
        public Task<Employment> FindByIdAsync(string employmentId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(employmentId))
                throw new ArgumentNullException(nameof(employmentId));
            return this._repositoryEmployment.FindByIdAsync(employmentId);
        }
        /*Поиск типа занятости по названию */
        public Task<Employment> FirstByNameAsync(string employmentName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(employmentName))
                throw new ArgumentNullException(nameof(employmentName));
            return this._repositoryEmployment.QueryableEntitySet.FirstOrDefaultAsync<Employment> (u => u.Name.ToUpper().Equals(employmentName.ToUpper()));
        }
        /*Создаем новый тип занятости*/
        public virtual async Task CreateAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));
            this._repositoryEmployment.Create(employment);

            await this._context.SaveChangesAsync();
        }
        /*Удаляем  тип занятости*/
        public virtual async Task DeleteAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));
            this._repositoryEmployment.Delete(employment);

            await this._context.SaveChangesAsync();
        }
        /*Обновляем изменненную тип занятости*/
        public virtual async Task UpdateAsync(Employment employment)
        {
            this.ThrowIfDisposed();
            if (employment == null)
                throw new ArgumentNullException(nameof(employment));
            this._repositoryEmployment.Update(employment);

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
                this._repositoryEmployment = null;
                this._disposed = true;
            }
        }
        /*Если был вызван Dispose то при попытке использования методов бросаем исключение*/
        private void ThrowIfDisposed()
        {
            if (this._disposed || _context.GetBoolDisposeContext)
                throw new ObjectDisposedException(this.GetType().Name);
        }
    }
}
