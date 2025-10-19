using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Views;

namespace IDGF.Core.Infrastructure.Repositories.Interface
{
    public interface ITransactionsRepository : ILDRCompatibleRepositoryAsync<Transactions, decimal>
    {
        Task<LinqDataResult<TransactionBasicView>> GetAllItemsView(
        LinqDataRequest request,
        int? bondId = null,
        int? brokerId = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null);

        Task<LinqDataResult<AggregatedTransactionReportItem>> GetAggregatedTransactionReportAsync(
            LinqDataRequest request,
            int? bondId = null,
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null);
        
        Task<List<AggregatedTransactionReportItem>> GetAggregatedTransactionReportForExportAsync(
            int? bondId = null,
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null);
    }
}
