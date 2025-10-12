using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace IDGF.Core.Infrastructure.Repositories.Implemention
{
    public class TransactionsRepository : LDRCompatibleRepositoryAsync<TransactionsEntity, Transactions, decimal>, ITransactionsRepository
    {
        private readonly CoreDbContext _context;
        public TransactionsRepository(CoreDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
