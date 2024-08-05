namespace SPMA.Models.Production
{
    public class InProductionXML
    {
        public string ComponentNumber { get; set; }
        public string WareCode { get; set; }
        public string WareName { get; set; }
        public string WareUnit { get; set; }
        public decimal? WareLength { get; set; }
        public int WareQuantity { get; set; }
        public decimal? ToIssue { get; set; }
        public decimal? Issued { get; set; }
        public decimal? TotalToIssue { get; set; } = 0;
        public decimal? Qavailable { get; set; } = 0;
        public sbyte QCheckStatus { get; set; } = 0;
    }
}
