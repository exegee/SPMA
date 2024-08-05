using System.Collections.Generic;

namespace SPMA.Dtos.Production
{
    public class ProductionSocketDto
    {
        public int ProductionSocketId { get; set; }

        public string Name { get; set; }
        public string Comment { get; set; }

        //public virtual ICollection<InProductionDto> InProduction { get; set; }
    }
}
