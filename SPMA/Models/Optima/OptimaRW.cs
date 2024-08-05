using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMA.Models.Optima
{
    public class OptimaRW
    {
        public int TrN_TrNID { get; set; }
        public string TrN_NumerPelny { get; set; }
        public sbyte TrN_Bufor { get; set; }
        [Column(TypeName = "decimal(15,2)")]
        public decimal? TrN_RazemNetto { get; set; }
        public string TrN_OpeModNazwisko { get; set; }
        public DateTime? TrN_DataDok { get; set; }
        public DateTime? TrN_DataOpe { get; set; }
    }
}
