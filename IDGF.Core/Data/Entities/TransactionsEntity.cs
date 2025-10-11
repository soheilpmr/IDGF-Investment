using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    public class TransactionsEntity : Transactions
    {
        public TransactionsEntity()
        {
                
        }

        public TransactionsEntity(Transactions transactions)
        {
            this.ID = transactions.ID;
            this.TransactionDate = transactions.TransactionDate;
            this.TransactionType = transactions.TransactionType;
            this.Quantity = transactions.Quantity;
            this.PricePerUnit = transactions.PricePerUnit;
            this.Commission = transactions.Commission;
            this.YtmAtTransaction = transactions.YtmAtTransaction;
            this.Status = transactions.Status;
            this.BondId = transactions.BondId;
            this.BrokerId = transactions.BrokerId;  
        }

        [ForeignKey(nameof(BondId))]
        public virtual BondsEntity? BondsEntity { get; set; }

        [ForeignKey(nameof(BrokerId))]
        public virtual BrokerageEntity? BrokerageEntity { get; set; }
    }
}
