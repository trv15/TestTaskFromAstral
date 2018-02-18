using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class Salary
    {
        /// <summary>
        /// связь один к одному с вакансией, Salary id - это id-вакансии
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("Vacancy")]
        public string Id { get; set; }
        /// <summary>
        /// зарплата от
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// зарплата до
        /// </summary>
        public string To { get; set; }
        /// <summary>
        /// удержаны ли налоги
        /// </summary>
        public string Gross { get; set; }
        /// <summary>
        /// валюта при расчете
        /// </summary>
        public string Currency { get; set; }

        public virtual Vacancy Vacancy { get; set; }
    }
}
