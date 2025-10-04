using BackEndInfrastructure.Infrastructure.Repository;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.Repositories.Implemention
{
    public class BondsTypeRepository : RepositoryAsync<BondsTypeEntity, BondsType, int>, IBondsTypeRepository
    {
        private readonly CoreDbContext _context;    
        public BondsTypeRepository(CoreDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
