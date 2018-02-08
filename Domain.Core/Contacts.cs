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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("Vacancy")]
        public string Id { get; set; }

        public string Name { get; set; }//фио контактное лицо

        public string Email { get; set; }//электронный адрес

        public virtual Vacancy Vacancy { get; set; }//один к одному

        public virtual ICollection<Phones> Phones { get; set; }//телефоны

        public Contacts()
        {
            this.Phones = (ICollection<Phones>)new List<Phones>();
        }
    }
}
