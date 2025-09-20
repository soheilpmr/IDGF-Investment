using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;

namespace IDGF.Core.Infrastructure.Repositories
{
    public class MandehTransactionsRepositories : LDRCompatibleRepositoryAsync<MandehTransactionsEntity, MandehTransactions, long>, IMandehTransactionsRepositories
    {
        private readonly CoreDbContext _context;    
        public MandehTransactionsRepositories(CoreDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
