using SPMA.Dtos.Production;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Production;
using SPMA.Models.Warehouse;

namespace SPMA.Dtos.Orders
{
    public class SubOrderDto
    {
        public int InProductionRWId { get; set; }
        // Assosiated production id
        public int InProductionId { get; set; }
        public InProductionDto InProduction { get; set; }

        // ASSOSIATED OPTIMA WARE
        public WareDto Ware { get; set; }
        public int? WareQuantity { get; set; }

        public decimal? WareLength { get; set; }
        public string WareUnit { get; set; }

        // Amount of componentWare multiplied by qty of book in main order
        public int ToIssue { get; set; }

        public int BookQty { get; set; }

        // Qty planned to cut
        public int PlannedToCut { get; set; }

        // Qty already issued to Optima
        public int Issued { get; set; }

        // Duplicates count
        public int TotalToIssue { get; set; }

        // Is additionaly purchasable
        public sbyte IsAdditionallyPurchasable { get; set; }

        // Is subOrder in Production
        public bool IsInProduction { get; set; }

        public bool IsBookQtyChanged { get; set; }

    }
}
