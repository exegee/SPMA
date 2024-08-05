using SPMA.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SPMA.Models.Production
{
    public class Book
    {
        public int BookId { get; set; }
        [Required]
        public string OfficeNumber { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        // BOOK TYPE
        // 0 - Inventor,
        // 1 - Manual
        public sbyte Type { get; set; }

        public string Comment { get; set; }
        public string Author { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }


        // VIRTUAL MEMBERS
        [IgnoreDataMember]
        public virtual ICollection<BookComponent> BookComponents { get; set; }
        [IgnoreDataMember]
        public virtual ICollection<OrderBook> OrderBooks { get; set; }
        //public virtual ICollection<InProduction> InProduction { get; set; }

    }
}
