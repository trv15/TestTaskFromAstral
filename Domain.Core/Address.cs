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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("Vacancy")]
        public string Id { get; set; }

        public string City { get; set; }//город

        public string Street { get; set; }//улица

        public string Building { get; set; }//данные здания

        public string Description { get; set; }//описание,уточнение

        public virtual Vacancy Vacancy { get; set; }//телефон
    }
}
