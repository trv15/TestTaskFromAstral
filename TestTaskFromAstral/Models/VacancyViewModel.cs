using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestTaskFromAstral.Models
{
    //основной обьект
    [JsonObject]
    public class MainObjectViewModel
    {
        public string Per_Page { get; set; }//кол. вакансии на странице

        public IList<VacancyViewModel> Items { get; set; }//список вакансии 

        public string Page { get; set; }//текущая страница

        public string Pages { get; set; }//всего страниц

        public string Found { get; set; }//всего вакансии
    }
    //модель вакансии для списка вакансии
    [JsonObject]
    public class VacancyListViewModel
    {
        public string Id { get; set; }//id из hh.ru

        public string Name { get; set; }//название вакансии

        public string Published_At { get; set; }//дата публикации

        public AddressViewModel Address { get; set; }//адресс

        public SalaryViewModel Salary { get; set; }//зарплата

        public TypeVacancyViewModel Type { get; set; }//тип вакансии(открытая закрытая)
    }
   
    
    
    //модель для подробного просмотра вакансии
    [JsonObject]
    public class VacancyViewModel
    {
        public string Id { get; set; }// id из hh.ru

        public string Name { get; set; }// название вакансии

        public string Published_At { get; set; }//дата публикации

        public string Description { get; set; }// описание вакансии

        public string Archived { get; set; }// описание вакансии

        public EmploymentViewModel Employment { get; set; }//тип занятости

        public TypeVacancyViewModel Type { get; set; }//тип вакансии(открытая закрытая)

        public SalaryViewModel Salary { get; set; }//зарплата

        public ContactsViewModel Contacts { get; set; }//контактные данные

        public AddressViewModel Address { get; set; }//адрес
    }       
    //модель для типа вакансии
    [JsonObject]
    public class TypeVacancyViewModel
    {
        public string Id { get; set; }//id из hh.ru(open или closed)

        public string Name { get; set; }//открытая, закрытая
    }
    //модель адресса
    [JsonObject]
    public class AddressViewModel
    {
        public string City { get; set; }//город

        public string Street { get; set; }//улица

        public string Building { get; set; }//данные здания

        public string Description { get; set; }//описание,уточнение
    }
    //тип занятости+
    [JsonObject]
    public class EmploymentViewModel
    {
        public string Id { get; set; }//id из hh.ru(full или др)

        public string Name { get; set; }//частиная полная
    }
    //зарплата от и до+
    [JsonObject]
    public class SalaryViewModel
    {
        public string From { get; set; }//от

        public string To { get; set; }//до

        public string Gross { get; set; }//налоги удержаны

        public string Currency { get; set; }//рубль или $
    }
    //контактное инфо+
    [JsonObject]
    public class ContactsViewModel
    {
        public string Name { get; set; }//фио контактноо лица

        public string Email { get; set; }//электронный адрес

        public IList<PhonesViewModel> Phones { get; set; }//коллекция телефонов
    }
    //модель телефона+
    [JsonObject]
    public class PhonesViewModel
    {
        public string Country { get; set; }//код страны

        public string City { get; set; }//код города

        public string Number { get; set; }//номер телефона

        public string Comment { get; set; }//комментарии(уточнение, требование)
    }

}