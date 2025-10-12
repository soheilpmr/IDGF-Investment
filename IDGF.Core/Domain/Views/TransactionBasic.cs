namespace IDGF.Core.Domain.Views
{
    public class TransactionBasic
    {
        public int Id { get; set; }                  // t.Id
        public int BondId { get; set; }              // b.Id
        public string Symbol { get; set; }           // b.Symbol
        public DateTime IssueDate { get; set; }      // b.IssueDate
        public DateTime MaturityDate { get; set; }   // b.MaturityDate
        public string BrokerName { get; set; }       // br.Name
        public decimal PricePerUnit { get; set; }    // t.PricePerUnit
        public int Quantity { get; set; }            // t.Quantity
        public decimal FaceValue { get; set; }       // b.FaceValue
        public DateTime TransactionDate { get; set; } // t.TransactionDate
        public decimal Commission { get; set; }      // t.Commission
        public short Status { get; set; }            // t.Status (assuming smallint)
        public string TransactionType { get; set; }  // t.TransactionType
    }
}
