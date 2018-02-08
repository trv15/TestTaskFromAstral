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
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("Vacancy")]
        public string Id { get; set; }

        public string From { get; set; }//от

        public string To { get; set; }//до

        public string Gross { get; set; }//налоги удержаны

        public string Currency { get; set; }//рубль или $

        public virtual Vacancy Vacancy { get; set; }//один к одному
    }
}
