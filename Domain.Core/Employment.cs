using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class Employment
    {
        /// <summary>
        ///id из hh.ru
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        ///частиная полная занятость 
        /// </summary>
        public string Name { get; set; }

        public virtual ICollection<Vacancy> Vacancy { get; set; }

        public Employment()
        {
            this.Vacancy = (ICollection<Vacancy>)new List<Vacancy>();
        }
    }
}
