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
    public class SalaryStore : ISharedStore<Salary, string>
    {
        private IRepositoryGeneric<Salary, string> _repositorySalary;
        public ApplicationContext _context { get; private set; }

        /*Конструктор*/
        public SalaryStore(ApplicationContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._repositorySalary = new RepositoryGeneric<Salary, string>(context);
        }
        /*Свойство для пользовательских запросов*/
        public IQueryable<Salary> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                return this._repositorySalary.QueryableEntitySet;
            }
        }
        /*Поиск зарплаты по Id*/
        public Task<Salary> FindByIdAsync(string Id)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(Id))
                throw new ArgumentNullException(nameof(Id));
            return this._repositorySalary.FindByIdAsync(Id);
        }
        /*Создаем новую зп*/
        public virtual async Task CreateAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));
            this._repositorySalary.Create(salary);

            await this._context.SaveChangesAsync();
        }
        /*Удаляем  запись зп*/
        public virtual async Task DeleteAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));
            this._repositorySalary.Delete(salary);

            await this._context.SaveChangesAsync();
        }
        /*Обновляем изменненную зп*/
        public virtual async Task UpdateAsync(Salary salary)
        {
            this.ThrowIfDisposed();
            if (salary == null)
                throw new ArgumentNullException(nameof(salary));
            this._repositorySalary.Update(salary);

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
                this._repositorySalary = null;
                this._disposed = true;
            }
        }
        /*Если был вызван Dispose то при попытке использования методов бросаем исключение*/
        private void ThrowIfDisposed()
        {
            if (this._disposed || _context.GetBoolDisposeContext)
                throw new ObjectDisposedException(this.GetType().Name);
        }



        public Task<Salary> FirstByNameAsync(string entityName)
        {
            throw new NotImplementedException();
        }
    }
}
