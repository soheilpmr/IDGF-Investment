using BackEndInfrsastructure.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace IDGF.Core.Domain
{
    public class CouponPayments : Model<decimal>
    {
        public decimal BondId { get; set; }
        public DateOnly PaymentDate { get; set; }
        public decimal AmountPerUnit { get; set; }
    }
}
