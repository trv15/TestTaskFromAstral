using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.Manager;

namespace Infrastructure.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        VacancyManager VacancyManager { get; }
        TypeVakancyManager TypeVakancyManager { get; }
        SalaryManager SalaryManager { get; }
        PhonesManager PhonesManager { get; }
        EmploymentManager EmploymentManager { get; }
        ContactsManager ContactsManager { get; }
        AddressManager AddressManagerv { get; }
        IApplicationContext getApplicationContext { get; }
        Task SaveAsync();
    }
}
