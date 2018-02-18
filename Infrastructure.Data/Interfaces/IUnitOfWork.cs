using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Data.Interfaces;
using Domain.Core;

namespace Infrastructure.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepositoryGeneric<Vacancy, string> VacancyRepository { get; }
        IRepositoryGeneric<TypeVacancy,string> TypeVakancyRepository { get; }
        IRepositoryGeneric<Salary, string> SalaryRepository { get; }
        IRepositoryGeneric<Phones, Guid> PhonesRepository { get; }
        IRepositoryGeneric<Employment, string> EmploymentRepository { get; }
        IRepositoryGeneric<Contacts, string> ContactsRepository { get; }
        IRepositoryGeneric<Address, string> AddressRepository { get; }
        IApplicationContext getApplicationContext { get; }
        Task SaveAsync();
    }
}
