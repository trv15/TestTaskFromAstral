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
    public class TypeVakancyStore : ISharedStore<TypeVakancy, string>
    {
        private IRepositoryGeneric<TypeVakancy, string> _repositoryTypeVakancy;
        public ApplicationContext _context { get; private set; }

        /*Конструктор*/
        public TypeVakancyStore(ApplicationContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._repositoryTypeVakancy = new RepositoryGeneric<TypeVakancy, string>(context);
        }
        /*Свойство для пользовательских запросов*/
        public IQueryable<TypeVakancy> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                return this._repositoryTypeVakancy.QueryableEntitySet;
            }
        }
        /*Поиск по Id*/
        public Task<TypeVakancy> FindByIdAsync(string vacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(vacancyId))
                throw new ArgumentNullException(nameof(vacancyId));
            return this._repositoryTypeVakancy.FindByIdAsync(vacancyId);
        }
        /*Поиск по названию */
        public Task<TypeVakancy> FirstByNameAsync(string typeVakancyName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(typeVakancyName))
                throw new ArgumentNullException(nameof(typeVakancyName));
            return this._repositoryTypeVakancy.QueryableEntitySet.FirstOrDefaultAsync<TypeVakancy>(u => u.Name.ToUpper().Equals(typeVakancyName.ToUpper()));
        }
        /*Создаем новый тип вакансии*/
        public virtual async Task CreateAsync(TypeVakancy typeVakancy)
        {
            this.ThrowIfDisposed();
            if (typeVakancy == null)
                throw new ArgumentNullException(nameof(typeVakancy));
            this._repositoryTypeVakancy.Create(typeVakancy);

            await this._context.SaveChangesAsync();
        }
        /*Удаляем  тип вакансии*/
        public virtual async Task DeleteAsync(TypeVakancy typeVakancy)
        {
            this.ThrowIfDisposed();
            if (typeVakancy == null)
                throw new ArgumentNullException(nameof(typeVakancy));
            this._repositoryTypeVakancy.Delete(typeVakancy);

            await this._context.SaveChangesAsync();
        }
        /*Обновляем изменненный тип вакансии*/
        public virtual async Task UpdateAsync(TypeVakancy typeVakancy)
        {
            this.ThrowIfDisposed();
            if (typeVakancy == null)
                throw new ArgumentNullException(nameof(typeVakancy));
            this._repositoryTypeVakancy.Update(typeVakancy);

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
                this._repositoryTypeVakancy = null;
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
