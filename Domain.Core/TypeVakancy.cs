using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class TypeVakancy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }//id из hh.ru(open closed)

        public string Name { get; set; }//открытая, закрытая

        public virtual ICollection<Vacancy> Vacancy { get; set; }//телефоны

        public TypeVakancy()
        {
            this.Vacancy = (ICollection<Vacancy>)new List<Vacancy>();
        }
    }
}
