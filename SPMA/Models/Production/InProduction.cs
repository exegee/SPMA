using SPMA.Models.Orders;
using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMA.Models.Production
{
    public class InProduction
    {
        public int InProductionId { get; set; }

        //public int OrderId { get; set; }
        //public Order Order { get; set; }

        //public int BookId { get; set; }
        //public Book Book { get; set; }

        public int OrderBookId { get; set; }
        public OrderBook OrderBook { get; set; }

        public int ComponentId { get; set; }
        public Component Component { get; set; }

        public int? BookOrderTree { get; set; }
        public int? BookLevelTree { get; set; }

        public int PlannedQty { get; set; }
        public int BookQty { get; set; }
        public int FinishedQty { get; set; } = 0;

        public int? ProductionSocketId { get; set; }
        public ProductionSocket ProductionSocket { get; set; }

        // PRODUCTION STATE OF THE COMPONENT
        // 0 - Idle ( not in production )
        // 1 - Awaiting for production
        public int ProductionStateId { get; set; }
        public ProductionState ProductionState { get; set; }

        // PRODUCTION TECHNOLOGY
        public string Technology { get; set; }

        // SOURCE TYPE
        // 0 - standard,
        // 1 - saw,
        // 2 - plasma IN,
        // 3 - plasma OUT without entrusted material,
        // 4 - plasma OUT with entrusted material,
        // 5 - purchase
        public sbyte? SourceType { get; set; }

        // ASSOSIATED OPTIMA WARE
        public Ware Ware { get; set; }

        public int? WareQuantity { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? WareLength { get; set; }
        public string WareUnit { get; set; }

        //// ADDITIONAL INFO
        //public int? level { get; set; }
        //public int? order { get; set; }

        // VIRTUAL MEMBERS
        public virtual ICollection<InProductionRW> InProductionRWs { get; set; }

        public int? ParentReservationId { get; set; }
        [ForeignKey("ParentReservationId")]
        public virtual InProduction ParentReservation { get; set; }

        public int? ReservedQty { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string ProductionTime { get; set; }
    }
}
