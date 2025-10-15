using BackEndInfrastructure.Infrastructure.Repository;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.Repositories.Implemention
{
    public class BrokerageRepository : RepositoryAsync<BrokerageEntity, Brokerage, int>, IBrokerageRepository
    {
        private readonly CoreDbContext _context;
        public BrokerageRepository(CoreDbContext coreDbContext) : base(coreDbContext)
        {
            _context = coreDbContext;
        }

        public async Task<IReadOnlyList<Brokerage>> GetAllForDropDown()
        {
            var result = _context.Brokerages.Select(ss => new Brokerage()
            {
                ID = ss.ID,
                Name = ss.Name
            }).ToList();

            return result;
        }
    }
}
