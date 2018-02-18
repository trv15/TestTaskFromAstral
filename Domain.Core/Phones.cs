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
    {   /// <summary>
        /// первичный ключ таблицы телефонные номера
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        /// <summary>
        ///код страны
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// код города
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// номер телефона
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// комментарии(уточнение, требование)
        /// </summary>
        public string Comment { get; set; }

        public string ContactsId { get; set; }
        public virtual Contacts Contacts { get; set; }
    }
}
