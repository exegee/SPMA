using SPMA.Models.Warehouse;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPMA.Models.Production
{
    public class InProductionRW
    {
        public int InProductionRWId { get; set; }

        // Assosiated production id
        public int InProductionId { get; set; }
        public InProduction InProduction { get; set; }

        // ASSOSIATED OPTIMA WARE
        public Ware Ware { get; set; }
        public int? WareQuantity { get; set; }
        [Column(TypeName = "decimal(18,3)")]
        public decimal? WareLength { get; set; }
        public string WareUnit { get; set; }

        // Amount of componentWare multiplied by qty of book in main order
        public int ToIssue { get; set; }

        // Qty planned to cut
        public int PlannedToCut { get; set; }

        // Qty already issued to Optima
        public int Issued { get; set; }

        // Duplicates count
        public int TotalToIssue { get; set; }

        // Is additionaly purchasable
        public sbyte IsAdditionallyPurchasable { get; set; }
    }
}
