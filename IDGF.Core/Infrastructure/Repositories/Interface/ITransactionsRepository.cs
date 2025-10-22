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

        Task<(double Mablagh, double DarRah, decimal TotalSumKhazaneKharid, decimal TotalSumeEjareDolatKharid, decimal TotalSumePartnershipBondKharid)>
             GetBondAndTransactionSummaryAsync(DateOnly dateOnly);
    }

    public class BondAndTransactionSummaryDto
    {
        /// <summary>
        /// مانده سرمایه گذاری ها
        /// </summary>
        public decimal InvestmentBalance { get; set; }//مانده سرمایه گذاری ها
        /// <summary>
        /// چک های در راه
        /// </summary>
        public double ChecksInTransit { get; set; } //چک های در راه
        /// <summary>
        /// جمع
        /// </summary>
        public decimal TotalSum { get; set; }//جمع
        /// <summary>
        /// مانده حساب تمرکز درآمد نزد بانک مرکزی
        /// </summary>
        public double IncomeConcentrationAccountBalanceWithTheCentralBank { get; set; }//مانده حساب تمرکز درآمد نزد بانک مرکزی
        /// <summary>
        /// مانده حساب تمرکز درآمد نزد بانک مرکزی
        /// </summary>
        public decimal CanbekeptwiththeCentralBank30 { get; set; }//مانده حساب تمرکز درآمد نزد بانک مرکزی
        /// <summary>
        /// درصد قابل سرمایه گذاری
        /// </summary>
        public decimal Investable70 { get; set; } //۷۰ درصد قابل سرمایه گذاری
        /// <summary>
        /// مازاد
        /// </summary>
        public decimal Surplus { get; set; } //مازاد

    }
}
