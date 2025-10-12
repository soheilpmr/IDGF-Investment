using BackEndInfrsastructure.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IDGF.Core.Domain
{
    public class Brokerage : Model<int>
    {
        public string Name { get; set; }
    }
}
