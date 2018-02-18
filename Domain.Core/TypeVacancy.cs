using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class TypeVacancy
    {
        /// <summary>
        ///id из hh.ru(open closed)
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        ///открытая, закрытая
        /// </summary>
        public string Name { get; set; }

        public virtual ICollection<Vacancy> Vacancy { get; set; }

        public TypeVacancy()
        {
            this.Vacancy = (ICollection<Vacancy>)new List<Vacancy>();
        }
    }
}
