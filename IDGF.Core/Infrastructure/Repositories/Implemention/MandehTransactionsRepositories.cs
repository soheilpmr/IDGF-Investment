using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;

namespace IDGF.Core.Infrastructure.Repositories.Implemention
{
    public class MandehTransactionsRepositories : LDRCompatibleRepositoryAsync<MandehTransactionsEntity, MandehTransactions, long>, IMandehTransactionsRepositories
    {
        private readonly CoreDbContext _context;    
        public MandehTransactionsRepositories(CoreDbContext context)
            : base(context)
        {
            _context = context;
        }

        public override async Task<LinqDataResult<MandehTransactions>> AllItemsAsync(LinqDataRequest request)
        {
            return await _context.MndehTransactions.OrderByDescending(ss => ss.TransactionDate)
                .ToLinqDataResultAsync<MandehTransactions>(request.Take, request.Skip, request.Sort, request.Filter);
        }
    }
}
