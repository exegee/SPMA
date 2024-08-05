using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMA.Dtos.Production
{
    public class ReservedItemDto
    {
        public int ReservedItemId { get; set; }

        public int InProductionRWId { get; set; }
        public InProductionRWDto InProductionRW { get; set; }

        public int WarehouseItemId { get; set; }
        public WarehouseItemDto WarehouseItem { get; set; }

        public int Quantity { get; set; }
    }
}
