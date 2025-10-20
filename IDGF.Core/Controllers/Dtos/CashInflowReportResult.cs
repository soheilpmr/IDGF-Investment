namespace IDGF.Core.Controllers.Dtos
{
    public class CashInflowReportResult
    {
        public decimal GrandTotal { get; set; }
        public List<CashInflowReportItem> Items { get; set; }
    }

    public class CashInflowReportItem
    {
        public string Symbol { get; set; }
        public DateOnly MaturityDate { get; set; }
        public DateOnly PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
