using SPMA.Dtos.Warehouse;
using SPMA.Models.Production;
using System;


namespace SPMA.Dtos.Production
{
    public class WarehouseItemDto
    {
        public int? WarehouseItemId { get; set; }
        public int ComponentId { get; set; }
        public ComponentDto Component { get; set; }
        public int WareId { get; set; }
        public WareDto Ware { get; set; }
        public int ComponentQty { get; set; }
        public int ReservedQty { get; set; }
        public decimal WareQty { get; set; }
        public decimal WareQtySum { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public string Comment { get; set; }
        public int State { get; set; }
        public InProductionRW ParentRWItem { get; set; }
    }
}
