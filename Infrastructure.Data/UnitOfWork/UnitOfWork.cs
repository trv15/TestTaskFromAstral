using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Context;
using Domain.Core;
using Infrastructure.Data.Repositories;

namespace Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public ApplicationContext _context;
        private IRepositoryGeneric<Vacancy, string> _vacancyRepository;
        private IRepositoryGeneric<TypeVacancy, string> _typeVakancyRepository;
        private IRepositoryGeneric<Salary, string> _salaryRepository;
        private IRepositoryGeneric<Phones, Guid> _phonesRepository;
        private IRepositoryGeneric<Employment, string> _employmentRepository;
        private IRepositoryGeneric<Contacts, string> _contactsRepository;
        private IRepositoryGeneric<Address, string> _addressRepository;

        public UnitOfWork(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            _context = (ApplicationContext)context;

             _vacancyRepository = new RepositoryGeneric<Vacancy, string>(_context);
             _typeVakancyRepository = new RepositoryGeneric<TypeVacancy, string>(_context);
             _salaryRepository = new RepositoryGeneric<Salary, string>(_context);
             _phonesRepository = new RepositoryGeneric<Phones, Guid>(_context);
             _employmentRepository = new RepositoryGeneric<Employment, string>(_context);
             _contactsRepository = new RepositoryGeneric<Contacts, string>(_context);
             _addressRepository = new RepositoryGeneric<Address, string>(_context);
        }

        #region Properties
        public IApplicationContext getApplicationContext
        {
            get
            {
                this.ThrowIfDisposed();
                if (_context == null)
                    throw new NotSupportedException();
                return _context;
            }
        }

        public IRepositoryGeneric<Vacancy, string> VacancyRepository
        {
            get
            {
                this.ThrowIfDisposed();
                if (_vacancyRepository == null || _context == null)
                    throw new NotSupportedException();

                return _vacancyRepository;
            }
        }

        public IRepositoryGeneric<TypeVacancy, string> TypeVakancyRepository
        {
            get
            {
                this.ThrowIfDisposed();
                if (_typeVakancyRepository == null || _context == null)
                    throw new NotSupportedException();

                return _typeVakancyRepository;
            }
        }

        public IRepositoryGeneric<Salary, string> SalaryRepository
        {
            get
            {
                this.ThrowIfDisposed();
                if (_salaryRepository == null || _context == null)
                    throw new NotSupportedException();

                return _salaryRepository;
            }
        }

        public IRepositoryGeneric<Phones, Guid> PhonesRepository
        {
            get
            {
                this.ThrowIfDisposed();
                if (_phonesRepository == null || _context == null)
                    throw new NotSupportedException();

                return _phonesRepository;
            }
        }

        public IRepositoryGeneric<Employment, string> EmploymentRepository
        {
            get
            {
                this.ThrowIfDisposed();
                if (_employmentRepository == null || _context == null)
                    throw new NotSupportedException();

                return _employmentRepository;
            }
        }

        public IRepositoryGeneric<Contacts, string> ContactsRepository
        {
            get
            {
                this.ThrowIfDisposed();
                if (_contactsRepository == null || _context == null)
                    throw new NotSupportedException();

                return _contactsRepository;
            }
        }

        public IRepositoryGeneric<Address, string> AddressRepository
        {
            get
            {
                this.ThrowIfDisposed();
                if (_addressRepository == null || _context == null)
                    throw new NotSupportedException();

                return _addressRepository;
            }
        }
        #endregion Properties
        /// <summary>
        /// Сохранить все изменения
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        #region Dispose
        /*Управление уничтожением объектов*/
        public void Dispose()
        {
            if (_disposed)
                return;

            DisposeFromAll(true);
            GC.SuppressFinalize(this);
        }
        private bool _disposed = false;
        private void DisposeFromAll(bool disposing)
        {
            if (!this._disposed)
            {

                if (_context.GetBoolDisposeContext == false && disposing)
                {
                    _context.Dispose();
                    _context = null;
                }
                _vacancyRepository = null;
                _typeVakancyRepository = null;
                _salaryRepository = null;
                _phonesRepository = null;
                _employmentRepository = null;
                _contactsRepository = null;
                _addressRepository = null;

                this._disposed = true;
            }
        }
        private void ThrowIfDisposed()
        {
            if (this._disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }
        #endregion Dispose
    }
}

