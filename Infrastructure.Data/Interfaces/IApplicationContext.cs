using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Core;
using System.Data.Entity;

namespace Infrastructure.Data.Interfaces
{
    public interface IApplicationContext : IDisposable
    {
         DbSet<Vacancy> Vacancys { get; set; }
         DbSet<TypeVakancy> TypeVakancys { get; set; }
         DbSet<Salary> Salarys { get; set; }
         DbSet<Employment> Employments { get; set; }
         DbSet<Contacts> Contacts { get; set; }
         DbSet<Phones> Phones { get; set; }
         DbSet<Address> Address { get; set; }
    }
}
