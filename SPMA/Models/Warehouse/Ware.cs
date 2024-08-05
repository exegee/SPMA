using SPMA.Models.Production;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMA.Models.Warehouse
{
    public class Ware
    {
        public int WareId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(3, 2)")]
        public decimal? Quantity { get; set; }
        public string Unit { get; set; }
        [Column(TypeName = "decimal(10, 10)")]
        public decimal? Converter { get; set; }
        public DateTime? Date { get; set; }
        public string TwG_Kod {get; set;}
        public string TwG_Nazwa { get; set; }
        public string Mag_Nazwa { get; set; }
        public string Mag_Symbol { get; set; }

        ////public virtual ICollection<Component> Components { get; set; }
        //public virtual ICollection<InProduction> InProduction { get; set; }
        //public virtual ICollection<BookComponent> BookComponents { get; set; }
    }
}
