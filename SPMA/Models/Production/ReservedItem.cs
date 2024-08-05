using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPMA.Models.Production
{
    public class ReservedItem
    {
       
        public int ReservedItemId { get; set; }

        //public int InProductionRWId { get; set; }
        public InProductionRW InProductionRW { get; set; }

        //public int WarehouseItemId { get; set; }
        public WarehouseItem WarehouseItem { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
