using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Views;
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

        /// <summary>
        /// Return vw_TransactionBasic 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="bondId"></param>
        /// <param name="brokerName"></param>
        /// <param name="transactionDateFrom"></param>
        /// <param name="transactionDateTo"></param>
        /// <returns></returns>
        public async Task<LinqDataResult<TransactionBasicView>> GetAllItemsView(
            LinqDataRequest request,
            int? bondId = null,
            string? brokerName = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            var query = _context.TransactionBasicViews.AsQueryable().AsNoTracking();

            // ✅ Optional filters (only apply if parameter has value)
            if (bondId.HasValue)
                query = query.Where(t => t.BondId == bondId.Value);

            if (!string.IsNullOrEmpty(brokerName))
                query = query.Where(t => t.BrokerName == brokerName);

            if (transactionDateFrom.HasValue)
                query = query.Where(t => t.TransactionDate >= transactionDateFrom.Value);

            if (transactionDateTo.HasValue)
                query = query.Where(t => t.TransactionDate <= transactionDateTo.Value);
            // ✅ Apply pagination, sorting, and filtering from your LINQ request
            var result = await query.ToLinqDataResultAsync<TransactionBasicView>(
                request.Take, request.Skip, request.Sort, request.Filter);

            return result;
        }
    }

 

}
