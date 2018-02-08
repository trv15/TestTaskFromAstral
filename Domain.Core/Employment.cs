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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }//id из hh.ru

        public string Name { get; set; }//частиная полная

        public virtual ICollection<Vacancy> Vacancy { get; set; }//телефоны

        public Employment()
        {
            this.Vacancy = (ICollection<Vacancy>)new List<Vacancy>();
        }
    }
}
