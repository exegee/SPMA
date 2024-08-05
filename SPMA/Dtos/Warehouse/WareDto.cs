using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SPMA.Dtos.Warehouse
{
    public class WareDto
    {
        public int? WareId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal? Length { get; set; }
        public string Unit { get; set; }
        public decimal Converter { get; set; }
        public DateTime? Date { get; set; }
        public string TwG_Kod { get; set; }
        public string TwG_Nazwa { get; set; }
        public string Mag_Nazwa { get; set; }
        public string Mag_Symbol { get; set; }

        //public ICollection<Component> Components { get; set; }
    }
}
