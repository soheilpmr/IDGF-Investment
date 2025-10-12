using IDGF.Core.Domain;

namespace IDGF.Core.Data.Entities
{
    public class BrokerageEntity:Brokerage
    {
        public ICollection<TransactionsEntity>? TransactionsEntities { get; set; }
    }
}
