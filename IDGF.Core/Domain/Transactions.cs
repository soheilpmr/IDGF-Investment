using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class Transactions : Model<decimal>
    {
        public DateOnly TransactionDate { get; set; }
        public string TransactionType { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Commission { get; set; }
        public decimal InvestmentPrice { get; set; }
        public decimal? YtmAtTransaction { get; set; }
        public short Status { get; set; }


        public decimal BondId { get; set; }
        public int BrokerId { get; set; }
    }
}
