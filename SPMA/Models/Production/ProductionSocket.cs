using System.Collections.Generic;

namespace SPMA.Models.Production
{
    public class ProductionSocket
    {
        public int ProductionSocketId { get; set; }

        public string Name { get; set; }
        public string Comment { get; set; }

        public virtual ICollection<InProduction> InProduction { get; set; }

    }
}
