using Newtonsoft.Json;
using SPMA.Dtos.Warehouse;
using System.Runtime.Serialization;

namespace SPMA.Dtos.Production
{
    public class InProductionRWDto
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
        public int ToIssue { get; set; } = 1;

        // Amount to issue per book
        public int ToIssuePerBook { get; set; } = 1;

        // Amount to issue per suborder
        public int ToIssuePerSubOrder { get; set; } = 1;

        // Qty planned to cut
        public int PlannedToCut { get; set; }

        // Qty already issued to Optima
        public int Issued { get; set; }

        // Duplicates count
        public int TotalToIssue { get; set; }

        // Differance between required and available in warehouse amount
        public decimal? QtyWhDiff { get; set; }

        public sbyte ProductionStateCode { get; set; }
    }
}
