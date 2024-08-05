using SPMA.Dtos.Orders;
using SPMA.Dtos.Warehouse;
using System;
using System.Collections.Generic;

namespace SPMA.Dtos.Production
{
    public class InProductionDto
    {
        public int? InProductionId { get; set; }

        public int? OrderBookId { get; set; }
        public OrderBookDto OrderBook { get; set; }

        public int? ComponentId { get; set; }
        public ComponentDto Component { get; set; }

        public BookDto Book { get; set; }

        public int PlannedQty { get; set; }
        public int BookQty { get; set; }
        public int FinishedQty { get; set; } = 0;

        public int? ProductionSocketId { get; set; }
        public ProductionSocketDto ProductionSocket { get; set; }

        // PRODUCTION STATE OF THE COMPONENT
        // 0 - Idle
        // 1 - Awaiting for production
        public int? ProductionStateId { get; set; }
        public ProductionStateDto ProductionState { get; set; }

        // PRODUCTION TECHNOLOGY
        public string Technology { get; set; }

        // SOURCE TYPE ???
        // 0 - standard,
        // 1 - saw,
        // 2 - plasma IN,
        // 3 - plasma OUT without entrusted material,
        // 4 - plasma OUT with entrusted material,
        // 5 - purchase
        public sbyte LastSourceType { get; set; }

        // SOURCE TYPE
        // 0 - standard,
        // 1 - saw,
        // 2 - plasma IN,
        // 3 - plasma OUT without entrusted material,
        // 4 - plasma OUT with entrusted material,
        // 5 - purchase
        public sbyte? SourceType { get; set; }

        // ASSOSIATED OPTIMA WARE
        public WareDto Ware { get; set; }

        public int? WareQuantity { get; set; }
        public decimal? WareLength { get; set; }
        public string WareUnit { get; set; }

        // VIRTUAL MEMBERS
        //public virtual ICollection<InProductionRWDto> InProductionRWs { get; set; }

        //public int? ParentReservationId { get; set; }
        //public virtual InProductionDto ParentReservation { get; set; }

        public int? ReservedQty { get; set; }

        // List of components to add
        public ComponentDto[] Components { get; set; }

        public string SubOrderNumber { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string ProductionTime { get; set; }

        // BOOK INFORMATION
        public int? BookOrderTree { get; set; }
        public int? BookLevelTree { get; set; }
    }
}
