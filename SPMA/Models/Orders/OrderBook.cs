using SPMA.Models.Production;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPMA.Models.Orders
{
    public class OrderBook
    {
        public int OrderBookId { get; set; }

        public int OrderId { get; set; }
        //[IgnoreDataMember]
        public Order Order { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public string Number { get; set; }

        public int PlannedQty { get; set; } = 1;
        public int FinishedQty { get; set; } = 0;

        public DateTime? AddedDate { get; set; }
        public string Comment { get; set; }
        public int? AddedById { get; set; }

        // Current state of the book
        //public int ProductionStateId { get; set; }
        public sbyte ProductionState { get; set; }

        public virtual ICollection<InProduction> InProduction { get; set; }

        public byte WareList { get; set; }
        public byte PurchaseList { get; set; }
        public byte PlasmaInList { get; set; }
        public byte PlasmaOutList { get; set; }
    }
}
