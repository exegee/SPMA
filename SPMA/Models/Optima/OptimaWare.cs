using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMA.Models.Optima
{
    public class OptimaWare
    {
        public int Twr_TwrId { get; set; }
        public byte Twr_NieAktywny { get; set; }
        public string TwG_Kod { get; set; }
        public string TwG_Nazwa { get; set; }
        public string Mag_Nazwa { get; set; } = "TYLKO KARTA TOWARU";
        public string Mag_Symbol { get; set; } = "TYLKO KARTA TOWARU";
        public string Twr_Kod { get; set; }
        public string Twr_Nazwa { get; set; }
        [Column(TypeName = "decimal(15,4)")]
        public decimal? TwI_Ilosc { get; set; } = 0;
        public string Twr_JM { get; set; }
        public decimal Twr_JmPomPrzelicznikL { get; set; }
        public DateTime? TwI_Data { get; set; }


        public OptimaWare() { }

        public OptimaWare(int twr_TwrId, byte twr_NieAktywny, string twG_Kod, string twG_Nazwa, string mag_Nazwa, string mag_Symbol, string twr_Kod,
            string twr_Nazwa, decimal? twI_Ilosc, string twr_JM, decimal twr_JmPomPrzelicznikL, DateTime? twI_Data) {
            Twr_TwrId = twr_TwrId;
            Twr_NieAktywny = twr_NieAktywny;
            TwG_Kod = twG_Kod;
            TwG_Nazwa = twG_Nazwa;
            Mag_Nazwa = mag_Nazwa;
            Mag_Symbol = mag_Symbol;
            Twr_Kod = twr_Kod;
            Twr_Nazwa = twr_Nazwa;
            TwI_Ilosc = twI_Ilosc;
            Twr_JM = twr_JM;
            Twr_JmPomPrzelicznikL = twr_JmPomPrzelicznikL;
            TwI_Data = twI_Data;
        }

        public OptimaWare(OptimaWare ware)
        {
            Twr_TwrId = ware.Twr_TwrId;
            Twr_NieAktywny = ware.Twr_NieAktywny;
            TwG_Kod = ware.TwG_Kod;
            TwG_Nazwa = ware.TwG_Nazwa;
            Mag_Nazwa = ware.Mag_Nazwa;
            Mag_Symbol = ware.Mag_Symbol;
            Twr_Kod = ware.Twr_Kod;
            Twr_Nazwa = ware.Twr_Nazwa;
            TwI_Ilosc = ware.TwI_Ilosc;
            Twr_JM = ware.Twr_JM;
            Twr_JmPomPrzelicznikL = ware.Twr_JmPomPrzelicznikL;
            TwI_Data = ware.TwI_Data;
        }
    }
}
