using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.Manager;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.Context;
using Domain.Core;
using Infrastructure.Data.Validator;

namespace Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public static ApplicationContext _context;
        private VacancyManager _vacancyManager;
        private TypeVakancyManager _typeVakancyManager;
        private SalaryManager _salaryManager;
        private PhonesManager _phonesManager;
        private EmploymentManager _employmentManager;
        private ContactsManager _contactsManager;
        private AddressManager _addressManager;

        public UnitOfWork(IApplicationContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            _context = (ApplicationContext)context;

            _vacancyManager = new VacancyManager(_context);
            _typeVakancyManager = new TypeVakancyManager(_context);
            _salaryManager = new SalaryManager(_context);
            _phonesManager = new PhonesManager(_context);
            _employmentManager = new EmploymentManager(_context);
            _contactsManager = new ContactsManager(_context);
            _addressManager = new AddressManager(_context);

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

        public VacancyManager VacancyManager
        {
            get
            {
                this.ThrowIfDisposed();
                if (_vacancyManager == null || _context == null)
                    throw new NotSupportedException();

                return _vacancyManager;
            }
        }
        
        public TypeVakancyManager TypeVakancyManager
        {
            get
            {
                this.ThrowIfDisposed();
                if (_typeVakancyManager == null || _context == null)
                    throw new NotSupportedException();

                return _typeVakancyManager;
            }
        }

        public SalaryManager SalaryManager
        {
            get
            {
                this.ThrowIfDisposed();
                if (_salaryManager == null || _context == null)
                    throw new NotSupportedException();

                return _salaryManager;
            }
        }

        public PhonesManager PhonesManager
        {
            get
            {
                this.ThrowIfDisposed();
                if (_phonesManager == null || _context == null)
                    throw new NotSupportedException();

                return _phonesManager;
            }
        }

        public EmploymentManager EmploymentManager
        {
            get
            {
                this.ThrowIfDisposed();
                if (_employmentManager == null || _context == null)
                    throw new NotSupportedException();

                return _employmentManager;
            }
        }

        public ContactsManager ContactsManager
        {
            get
            {
                this.ThrowIfDisposed();
                if (_contactsManager == null || _context == null)
                    throw new NotSupportedException();

                return _contactsManager;
            }
        }

        public AddressManager AddressManagerv
        {
            get
            {
                this.ThrowIfDisposed();
                if (_addressManager == null || _context == null)
                    throw new NotSupportedException();

                return _addressManager;
            }
        }
        #endregion Properties

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
                if (disposing)
                {
                    if (_context != null)
                        _context = null;
                    _vacancyManager.Dispose();
                    _typeVakancyManager.Dispose();
                    _salaryManager.Dispose();
                    _phonesManager.Dispose();
                    _employmentManager.Dispose();
                    _contactsManager.Dispose();
                    _addressManager.Dispose();

                    _vacancyManager = null;
                    _typeVakancyManager = null;
                    _salaryManager = null;
                    _phonesManager = null;
                    _employmentManager = null;
                    _contactsManager = null;
                    _addressManager = null;
                }
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

