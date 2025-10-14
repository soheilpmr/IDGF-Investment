namespace IDGF.Core.Infrastructure
{
    public class AggregatedTransactionReportItem
    {
        public string Symbol { get; set; }
        public DateOnly DateOfMaturity { get; set; }
        public decimal Quantity { get; set; }
        public decimal? TotalPurchasePrice { get; set; } 
        public decimal TotalFaceValue { get; set; }
    }
}
