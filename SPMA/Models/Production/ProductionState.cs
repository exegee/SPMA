using SPMA.Models.Orders;
using System.Collections.Generic;

namespace SPMA.Models.Production
{
    public class ProductionState
    {
        public int ProductionStateId { get; set; }
        public sbyte ProductionStateCode { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }

        public virtual ICollection<InProduction> InProduction { get; set; }

        public virtual ICollection<OrderBook> OrderBooks { get; set; }
    }
}
