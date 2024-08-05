using SPMA.Dtos.Orders;
using System.Collections.Generic;

namespace SPMA.Dtos.Production
{
    public class ProductionStateDto
    {
        public int ProductionStateId { get; set; }
        public sbyte ProductionStateCode { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }

        //public virtual ICollection<InProductionDto> InProduction { get; set; }

        //public virtual ICollection<OrderBookDto> OrderBooks { get; set; }
    }
}
