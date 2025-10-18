using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{

    public class TransactionsEntity : Transactions
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ✅ important : for multipleInstance Adding
        public override decimal ID { get => base.ID; set => base.ID = value; }
        public TransactionsEntity()
        {
                
        }

        public TransactionsEntity(Transactions transactions)
        {
            this.TransactionDate = transactions.TransactionDate;
            this.TransactionType = transactions.TransactionType;
            this.Quantity = transactions.Quantity;
            this.PricePerUnit = transactions.PricePerUnit;
            this.Commission = transactions.Commission;
            this.YtmAtTransaction = transactions.YtmAtTransaction;
            this.Status = transactions.Status;
            this.BondId = transactions.BondId;
            this.BrokerId = transactions.BrokerId;  
            this.InvestmentPrice = transactions.InvestmentPrice;  
        }

        [ForeignKey(nameof(BondId))]
        public virtual BondsEntity? BondsEntity { get; set; }

        [ForeignKey(nameof(BrokerId))]
        public virtual BrokerageEntity? BrokerageEntity { get; set; }
    }
}
