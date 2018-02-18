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
        /// <summary>
        /// id вакансии устанавливается из hh.ru
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// название вакансии
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// дата публикации
        /// </summary>
        public string Published_At { get; set; }
        /// <summary>
        /// описание вакансии
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// в архиве ли вакансия
        /// </summary>
        public string Archived { get; set; }
        /// <summary>
        ///тип вакансии(открытая закрытая)
        /// </summary>
        public string TypeVakancyId { get; set; }
        public virtual TypeVacancy TypeVakancy { get; set; }
        /// <summary>
        ///тип занятости
        /// </summary>
        public string EmploymentId { get; set; }
        public virtual Employment Employment { get; set; }
        /// <summary>
        ///зарплата указанная
        /// </summary>
        public virtual Salary Salary { get; set; }
        /// <summary>
        ///контактные данные
        /// </summary>
        public virtual Contacts Contacts { get; set; }
        /// <summary>
        ///адрес
        /// </summary>
        public virtual Address Address { get; set; }
    }
}
