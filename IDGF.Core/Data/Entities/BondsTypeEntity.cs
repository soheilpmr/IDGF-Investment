using IDGF.Core.Domain;

namespace IDGF.Core.Data.Entities
{
    public class BondsTypeEntity : BondsType
    {
        //One To Many Relation with Bonds
        public virtual ICollection<BondsEntity>? BondsEntities { get; set; }
    }
}
