using SPMA.Dtos.Production;
using System;

namespace SPMA.Dtos.Orders
{
    public class OrderBookDto
    {
        public int? OrderBookId { get; set; }
        public int? OrderId { get; set; }
        public OrderDto Order { get; set; }

        public int? BookId { get; set; }
        public BookDto Book { get; set; }

        public string Number { get; set; }

        public int PlannedQty { get; set; } = 1;
        public int FinishedQty { get; set; } = 0;

        public DateTime? AddedDate { get; set; }
        public string Comment { get; set; }
        public int? AddedById { get; set; }

        public string Name { get; set; }
        public string OfficeNumber { get; set; }
        public sbyte Type { get; set; }
        public int? Position { get; set; }

        public string ComponentNumber { get; set; }
        public string OrderNumber { get; set; }

        public byte WareList { get; set; }
        public byte PurchaseList { get; set; }
        public byte PlasmaInList { get; set; }
        public byte PlasmaOutList { get; set; }
    }
}