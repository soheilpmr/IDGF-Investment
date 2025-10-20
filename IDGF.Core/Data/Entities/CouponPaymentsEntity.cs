using IDGF.Core.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Data.Entities
{
    public class CouponPaymentsEntity : CouponPayments
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override decimal ID { get => base.ID; set => base.ID = value; }
        public CouponPaymentsEntity()
        {

        }

        public CouponPaymentsEntity(CouponPayments couponPayments)
        {
            this.BondId = couponPayments.BondId;
            this.PaymentDate = couponPayments.PaymentDate;
            this.AmountPerUnit = couponPayments.AmountPerUnit;
        }

        [ForeignKey(nameof(BondId))]
        public virtual BondsEntity? BondsEntity { get; set; }
    }
}
