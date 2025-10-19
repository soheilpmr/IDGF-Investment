namespace IDGF.Core.Controllers.Dtos
{

    public class InvestmentReportResult
    {
        public decimal? GrandTotal { get; set; }
        public List<InvestmentReportItem> Items { get; set; }
    }
    public class InvestmentReportItem
    {
        public DateOnly TransactionDate { get; set; }
        public decimal? TotalInvestmentAmount { get; set; }
    }
}
