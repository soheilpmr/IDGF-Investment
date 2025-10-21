using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Controllers.Dtos;
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

        Task<List<TransactionBasicView>> GetAllItemsReportForExport(
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

        Task<InvestmentReportResult> GetInvestmentReportAsync(
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null);

        Task<CashInflowReportResult> GetCashInflowReportAsync(
            DateOnly? dateFrom = null,
            DateOnly? dateTo = null);

        Task<BondAndTransactionSummaryDto> GetBondAndTransactionSummaryAsync(DateOnly dateOnly);
    }

    public class BondAndTransactionSummaryDto
    {
        public decimal InvestmentBalance { get; set; }//مانده سرمایه گذاری ها
        public double ChecksInTransit { get; set; } //چک های در راه
        public decimal TotalSum { get; set; }//جمع
        public double IncomeConcentrationAccountBalanceWithTheCentralBank { get; set; }//مانده حساب تمرکز درآمد نزد بانک مرکزی
        public decimal CanbekeptwiththeCentralBank30 { get; set; }//مانده حساب تمرکز درآمد نزد بانک مرکزی
        public decimal Investable70 { get; set; } //۷۰ درصد قابل سرمایه گذاری
    }
}
