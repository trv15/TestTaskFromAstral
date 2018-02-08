using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class Vacancy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }// id из hh.ru

        public string Name { get; set; }// название вакансии

        public string Published_At { get; set; }//дата публикации

        public string Description { get; set; }// описание вакансии

        public string Archived { get; set; }// описание вакансии

        public string TypeVakancyId { get; set; }
        public virtual TypeVakancy TypeVakancy { get; set; }//тип вакансии(открытая закрытая)

        public string EmploymentId { get; set; }
        public virtual Employment Employment { get; set; }//тип занятости


        public virtual Address Address { get; set; }//адрес

        public virtual Salary Salary { get; set; }//зарплата

        public virtual Contacts Contacts { get; set; }//контактные данные
    }
}
