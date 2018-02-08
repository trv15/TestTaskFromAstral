using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Core
{
    public class Phones
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Country { get; set; }//код страны

        public string City { get; set; }//код города

        public string Number { get; set; }//номер телефона

        public string Comment { get; set; }//комментарии(уточнение, требование)

        public string ContactsId { get; set; }
        public virtual Contacts Contacts { get; set; }
    }
}
