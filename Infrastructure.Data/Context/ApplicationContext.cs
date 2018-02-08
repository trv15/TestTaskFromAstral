using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Domain.Core;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using Infrastructure.Data.Interfaces;

namespace Infrastructure.Data.Context
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        /*Статический контруктор инициилизирующий БД*/
        static ApplicationContext()
        {
            Database.SetInitializer<ApplicationContext>(new DbInitializer());
        }

        /*Конструктор по умолчанию*/
        public ApplicationContext() : base("ApplicationContext") { }

        public DbSet<Vacancy> Vacancys { get; set; }
        public DbSet<TypeVakancy> TypeVakancys { get; set; }
        public DbSet<Salary> Salarys { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<Contacts> Contacts { get; set; }
        public DbSet<Phones> Phones { get; set; }
        public DbSet<Address> Address { get; set; }

        /*Получаем вновь созданный контекст данных(использование может привести к ошибкам в если получить несколько экземпляров)*/
        public static ApplicationContext getNewAppContext()
        {
            return new ApplicationContext();
        }

        /*Переопределяем логику сопоставления моделей(классов) и таблиц Entity Framework с помощью FluentApi*/
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));
            /*Настраиваем имена таблиц, с которой будет сопоставлен конкретный тип сущности, методом ToTable()
             и свойства первичного ключа для выбранного типа сущности методом HasKey()*/
            EntityTypeConfiguration<Vacancy> _VacancyTable = modelBuilder.Entity<Vacancy>().HasKey(u => u.Id).ToTable("VacancysHeadHunter");
            EntityTypeConfiguration<TypeVakancy> _TypeVakancyTable = modelBuilder.Entity<TypeVakancy>().HasKey(i => i.Id).ToTable("TypeVakancys");
            EntityTypeConfiguration<Salary> _SalaryTable = modelBuilder.Entity<Salary>().HasKey(r => r.Id).ToTable("Salarys");//
            EntityTypeConfiguration<Employment> _EmploymentTable = modelBuilder.Entity<Employment>().HasKey(x => x.Id).ToTable("Employments");
            EntityTypeConfiguration<Contacts> _ContactsTable = modelBuilder.Entity<Contacts>().HasKey(z => z.Id).ToTable("Contacts");//
            EntityTypeConfiguration<Phones> _PhonesTable = modelBuilder.Entity<Phones>().HasKey(k => k.Id).ToTable("Phones");
            EntityTypeConfiguration<Address> _AddressTable = modelBuilder.Entity<Address>().HasKey(l => l.Id).ToTable("Address");//

            _VacancyTable.Property(u => u.Id).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("VakancyIdIndex") { IsUnique = true }));
            _TypeVakancyTable.Property(u => u.Id).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("TypeVakancyIdIndex") { IsUnique = true }));
            _EmploymentTable.Property(u => u.Id).IsRequired().HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute("EmploymentIdIndex") { IsUnique = true }));
        }

        /*Добавляем функционал по валидации*/
        protected override DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            if (entityEntry != null && entityEntry.State == EntityState.Added)
            {
                List<DbValidationError> source = new List<DbValidationError>();

                Vacancy vacancy = entityEntry.Entity as Vacancy;
                if (vacancy != null)
                {
                    if (this.Vacancys.Any<Vacancy>(u => string.Equals(u.Id, vacancy.Id)))
                        source.Add(new DbValidationError("Свойство " + nameof(vacancy.Id) + " вызвало ошибку, метод \"ValidateEntity\" класса \"ApplicationContext\"",
                            "Описание: Вакансия с ID: " + vacancy.Id + " уже сохранена в БД"));
                }

                TypeVakancy typeVakancy = entityEntry.Entity as TypeVakancy;
                if (typeVakancy != null)
                {
                    if (this.TypeVakancys.Any<TypeVakancy>(u => string.Equals(u.Id, typeVakancy.Id)))
                        source.Add(new DbValidationError("Свойство " + nameof(typeVakancy.Id) + " вызвало ошибку, метод \"ValidateEntity\" класса \"ApplicationContext\"",
                            "Описание: Тип вакансии " + vacancy.Id + " уже сохранена в БД"));
                }

                Employment employment = entityEntry.Entity as Employment;
                if (employment != null)
                {
                    if (this.Employments.Any<Employment>(u => string.Equals(u.Id, employment.Id)))
                        source.Add(new DbValidationError("Свойство " + nameof(employment.Id) + " вызвало ошибку, метод \"ValidateEntity\" класса \"ApplicationContext\"",
                            "Описание: Тип занятости " + vacancy.Id + " уже сохранена в БД"));
                }

                //Если коллекция содержит элементы(ошибки) то возвращаем результат валидации
                if (source.Any<DbValidationError>())
                    return new DbEntityValidationResult(entityEntry, (IEnumerable<DbValidationError>)source);
            }
            //Если сущность не принадлежит к типу пользователя и роли, применяется базовая логика валидации
            return base.ValidateEntity(entityEntry, items);
        }

        /*Проводим первичную инициализвцию БД*/
        public class DbInitializer : DropCreateDatabaseIfModelChanges<ApplicationContext>
        {
            protected override void Seed(ApplicationContext DB)
            {
                
            }
        }

        #region Dispose
        /*Реализация интерфейса IDisposable.*/
        public new void Dispose()
        {
            if (_disposed)
                return;
            DisposeFromAll(true);
            //Подавляем финализацию
            GC.SuppressFinalize(this);
        }

        private bool _disposed = false;

        public bool GetBoolDisposeContext
        {
            get
            {
                return this._disposed;
            }
        }

        private void DisposeFromAll(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //Освобождаем управляемые ресурсы
                    this.Vacancys = null;
                    this.TypeVakancys = null;
                    this.Salarys = null;
                    this.Employments = null;
                    this.Contacts = null;
                    this.Phones = null;
                    this.Address = null;
                    base.Dispose();
                }
                this._disposed = true;
            }
        }
        #endregion Dispose
    }
}
