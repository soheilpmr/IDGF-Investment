namespace IDGF.Core.Domain.Views
{
    public class TransactionBasicView
    {
        public decimal Id { get; set; }                  // t.Id
        public decimal BondId { get; set; }              // b.Id
        public decimal BondTypeId { get; set; }              // b.TypeId
        public string Symbol { get; set; }           // b.Symbol
        public DateOnly? IssueDate { get; set; }      // b.IssueDate
        public DateOnly MaturityDate { get; set; }   // b.MaturityDate
        public string? BrokerName { get; set; }       // br.Name
        public int? BrokerId { get; set; }       // br.ID
        public decimal PricePerUnit { get; set; }    // t.PricePerUnit
        public decimal Quantity { get; set; }            // t.Quantity
        public decimal FaceValue { get; set; }       // b.FaceValue
        public DateOnly TransactionDate { get; set; } // t.TransactionDate تاریخ سرمایه گذاری
        public decimal Commission { get; set; }      // t.Commission
        public short Status { get; set; }            // t.Status (assuming smallint)
        public string StatusText { get; set; }            // t.[StatusText] (assuming smallint)
        public string TransactionType { get; set; }  // t.TransactionType
        public decimal? InvestmentPrice { get; set; }  // t.InvestmentPrice,
    }
}
