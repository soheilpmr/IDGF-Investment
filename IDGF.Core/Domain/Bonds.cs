using BackEndInfrsastructure.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IDGF.Core.Domain
{
    public class Bonds : Model<decimal>
    {
        public int TypeID { get; set; }
        public string? Symbol { get; set; }
        public DateOnly? IssueDate { get; set; }
        public DateOnly MaturityDate { get; set; }
        public decimal FaceValue { get; set; }
        public decimal? CouponRatePercen { get; set; }
    }
}
