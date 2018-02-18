using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class Address
    {
        /// <summary>
        /// связи один к одному, Address id - это id вакансии
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("Vacancy")]
        public string Id { get; set; }
        /// <summary>
        /// город
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// улица
        /// </summary>
        public string Street { get; set; }
        /// <summary>
        /// номер дома кв и корпуса
        /// </summary>
        public string Building { get; set; }
        /// <summary>
        /// описание,уточнение для адреса
        /// </summary>
        public string Description { get; set; }

        public virtual Vacancy Vacancy { get; set; }
    }
}
