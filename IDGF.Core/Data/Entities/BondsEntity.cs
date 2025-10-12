using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    public class BondsEntity : Bonds
    {

        public BondsEntity()
        {
        }

        public BondsEntity(Bonds bonds)
        {
            this.TypeID = bonds.TypeID;
            this.Symbol = bonds.Symbol; 
            this.IssueDate = bonds.IssueDate;
            this.MaturityDate = bonds.MaturityDate;
            this.FaceValue = bonds.FaceValue;
            this.CouponRatePercen = bonds.CouponRatePercen;

        }

        [ForeignKey(nameof(TypeID))]
        public virtual BondsTypeEntity? BondTypesEntity { get; set; }

        public ICollection<TransactionsEntity>? TransactionsEntities { get; set; }
    }
}
