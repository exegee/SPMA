using SPMA.Models.Production;
using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPMA.Models.Warehouse
{
    public class WarehouseItem
    {
        public int WarehouseItemId { get; set; }
        [Required]
        public int ComponentId { get; set; }
        public Component Component { get; set; }    // 303.10.001
        [Required]
        public int WareId { get; set; }
        public Ware Ware { get; set; }              // Katownik
        [Required]
        public int ComponentQty { get; set; }       // 100 szt.
        public int? ReservedQty { get; set; } = 0;      //ilosc zarezerwowane w tabeli ReservedItems
        public decimal? WareQty { get; set; } = 0;       // 0.5 mb
        public decimal? WareQtySum { get; set; } = 0;   // ComponentQty * WareQty = 50mb
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public string Comment { get; set; }
        public int State { get; set; }
        public int? ParentRWItemId { get; set; }
       // [ForeignKey("ParentRWItemId")]
        public InProductionRW ParentRWItem { get; set; }

    }
}
