using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    public class BrokerageEntity:Brokerage
    {
        public ICollection<TransactionsEntity>? TransactionsEntities { get; set; }
    public class BrokerageEntity : Brokerage
    {
        public BrokerageEntity()
        {

        }

        public BrokerageEntity(Brokerage brokerage)
        {
            this.Name = brokerage.Name;
        }

    }
}
