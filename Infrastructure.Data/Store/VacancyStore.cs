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
    public class VacancyStore : ISharedStore<Vacancy, string>
    {
        private IRepositoryGeneric<Vacancy, string> _repositoryVacancy;
        public ApplicationContext _context { get; private set; }
        
        /*Конструктор*/
        public VacancyStore(ApplicationContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._repositoryVacancy = new RepositoryGeneric<Vacancy, string>(context);
        }
        /*Свойство для пользовательских запросов*/
        public IQueryable<Vacancy> QueryableSet
        {
            get
            {
                this.ThrowIfDisposed();
                return this._repositoryVacancy.QueryableEntitySet;
            }
        }
        /*Поиск вакансии по Id*/
        public Task<Vacancy> FindByIdAsync(string vacancyId)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrEmpty(vacancyId))
                throw new ArgumentNullException(nameof(vacancyId));

            return this._repositoryVacancy.FindByIdAsync(vacancyId);
        }
        /*Поиск по названию вакансии*/
        public Task<Vacancy> FirstByNameAsync(string vacancyName)
        {
            this.ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(vacancyName))
                throw new ArgumentNullException(nameof(vacancyName));
            return this._repositoryVacancy.QueryableEntitySet.FirstOrDefaultAsync<Vacancy>(u => u.Name.ToUpper().Equals(vacancyName.ToUpper()));
        }
        /*Создаем новую вакансию*/
        public virtual async Task CreateAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));
            this._repositoryVacancy.Create(vacancy);

            await this._context.SaveChangesAsync();
        }
        /*Удаляем  вакансию*/
        public virtual async Task DeleteAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));
            this._repositoryVacancy.Delete(vacancy);

            await this._context.SaveChangesAsync();
        }
        /*Обновляем изменненную вакансию*/
        public virtual async Task UpdateAsync(Vacancy vacancy)
        {
            this.ThrowIfDisposed();
            if (vacancy == null)
                throw new ArgumentNullException(nameof(vacancy));
            this._repositoryVacancy.Update(vacancy);

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
                this._repositoryVacancy = null;
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
