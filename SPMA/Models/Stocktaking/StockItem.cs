using System;

namespace SPMA.Models.Stocktaking
{
    public class StockItem
    {
        public int StockItemId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public sbyte Type { get; set; }
        public decimal PitQty { get; set; }
        public decimal ActualQty { get; set; }
        public decimal DiffQty { get; set; }
        public string Unit { get; set; }
        public string Comment { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
