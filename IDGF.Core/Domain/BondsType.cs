using BackEndInfrsastructure.Domain;

namespace IDGF.Core.Domain
{
    public class BondsType : Model<int>
    {
        public string Name { get; set; }
        public bool HasCoupon { get; set; }

    }
}
