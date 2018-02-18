using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class Contacts
    {
        /// <summary>
        /// связь один к одному с вакансией, Contacts id - это id вакансии
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("Vacancy")]
        public string Id { get; set; }
        /// <summary>
        ///фио контактное лицо
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///электронный адрес
        /// </summary>
        public string Email { get; set; }

        public virtual Vacancy Vacancy { get; set; }

        public virtual ICollection<Phones> Phones { get; set; }

        public Contacts()
        {
            this.Phones = (ICollection<Phones>)new List<Phones>();
        }
    }
}
