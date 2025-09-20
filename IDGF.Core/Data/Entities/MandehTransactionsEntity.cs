using IDGF.Core.Domain;

namespace IDGF.Core.Data.Entities
{
    public class MandehTransactionsEntity : MandehTransactions
    {
        public MandehTransactionsEntity()
        {
            
        }

        public MandehTransactionsEntity(MandehTransactions mandehTransactions)
        {
            this.DarRah = mandehTransactions.DarRah;
            this.Hazineh = mandehTransactions.Hazineh;
            this.Taeed = mandehTransactions.Taeed;
            this.Mablagh = mandehTransactions.Mablagh;
            this.TransactionTime = mandehTransactions.TransactionTime;
            this.TransactionDateTime = mandehTransactions.TransactionDateTime;
            this.TransactionDate = mandehTransactions.TransactionDate;
        }
    }
}
