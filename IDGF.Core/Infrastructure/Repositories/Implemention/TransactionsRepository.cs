using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Enums;
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
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            var query = _context.TransactionBasicViews.AsQueryable().AsNoTracking();

            // ✅ Optional filters (only apply if parameter has value)
            if (bondId.HasValue)
                query = query.Where(t => t.BondId == bondId.Value);

            if (brokerId.HasValue)
                query = query.Where(t => t.BrokerId == brokerId);

            if (transactionDateFrom.HasValue)
                query = query.Where(t => t.TransactionDate >= transactionDateFrom.Value);

            if (transactionDateTo.HasValue)
                query = query.Where(t => t.TransactionDate <= transactionDateTo.Value);
            // ✅ Apply pagination, sorting, and filtering from your LINQ request
            var result = await query.ToLinqDataResultAsync<TransactionBasicView>(
                request.Take, request.Skip, request.Sort, request.Filter);

            return result;
        }
        
        public async Task<List<TransactionBasicView>> GetAllItemsReportForExport(
            int? bondId = null,
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            var query = _context.TransactionBasicViews.AsQueryable().AsNoTracking();

            if (bondId.HasValue)
                query = query.Where(t => t.BondId == bondId.Value);

            if (brokerId.HasValue)
                query = query.Where(t => t.BrokerId == brokerId.Value);

            if (transactionDateFrom.HasValue)
                query = query.Where(t => t.TransactionDate >= transactionDateFrom.Value);

            if (transactionDateTo.HasValue)
                query = query.Where(t => t.TransactionDate <= transactionDateTo.Value);

            var result = await query.ToListAsync<TransactionBasicView>();

            return result;
        }

        public async Task<LinqDataResult<AggregatedTransactionReportItem>> GetAggregatedTransactionReportAsync(
            LinqDataRequest request,
            int? bondId = null,
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            var query = _context.TransactionBasicViews.AsQueryable().AsNoTracking();

            var filteredQuery = query.Where(t =>
                t.TransactionType == "Buy" &&
                t.Status == 2);

            if (bondId.HasValue)
                filteredQuery = filteredQuery.Where(t => t.BondId == bondId.Value);

            if (brokerId.HasValue)
                filteredQuery = filteredQuery.Where(t => t.BrokerId == brokerId.Value);

            if (transactionDateFrom.HasValue)
                filteredQuery = filteredQuery.Where(t => t.TransactionDate >= transactionDateFrom.Value);

            if (transactionDateTo.HasValue)
                filteredQuery = filteredQuery.Where(t => t.TransactionDate <= transactionDateTo.Value);
            var groupedQuery = filteredQuery.GroupBy(
                t => new { t.Symbol, t.MaturityDate },
                (key, group) => new AggregatedTransactionReportItem
                {
                    Symbol = key.Symbol,
                    DateOfMaturity = key.MaturityDate,
                    Quantity = group.Sum(x => x.Quantity),
                    TotalPurchasePrice = group.Sum(x => x.InvestmentPrice),
                    TotalFaceValue = group.Sum(x => x.Quantity * x.FaceValue)
                });

            var result = await groupedQuery.ToLinqDataResultAsync<AggregatedTransactionReportItem>(
                request.Take, request.Skip, request.Sort, request.Filter);

            return result;
        }

        public async Task<List<AggregatedTransactionReportItem>> GetAggregatedTransactionReportForExportAsync(
            int? bondId = null,
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            var query = _context.TransactionBasicViews.AsQueryable().AsNoTracking();

            var filteredQuery = query.Where(t =>
                t.TransactionType == "Buy" &&
                t.Status == 2);

            if (bondId.HasValue)
                filteredQuery = filteredQuery.Where(t => t.BondId == bondId.Value);

            if (brokerId.HasValue)
                filteredQuery = filteredQuery.Where(t => t.BrokerId == brokerId.Value);

            if (transactionDateFrom.HasValue)
                filteredQuery = filteredQuery.Where(t => t.TransactionDate >= transactionDateFrom.Value);

            if (transactionDateTo.HasValue)
                filteredQuery = filteredQuery.Where(t => t.TransactionDate <= transactionDateTo.Value);

            var groupedQuery = filteredQuery.GroupBy(
                t => new { t.Symbol, t.MaturityDate },
                (key, group) => new AggregatedTransactionReportItem
                {
                    Symbol = key.Symbol,
                    DateOfMaturity = key.MaturityDate,
                    Quantity = group.Sum(x => x.Quantity),
                    TotalPurchasePrice = group.Sum(x => x.InvestmentPrice),
                    TotalFaceValue = group.Sum(x => x.Quantity * x.FaceValue)
                });
            var result = await groupedQuery.ToListAsync();

            return result;
        }


        public async Task<InvestmentReportResult> GetInvestmentReportAsync(
            DateOnly? transactionDateTo,
            DateOnly? transactionDateFrom = null)
        {
            var query = _context.TransactionBasicViews.AsNoTracking();

            var filteredQuery = query.Where(t =>

                t.TransactionType == "Buy" &&

                t.Status == 2 &&
                t.TransactionDate <= transactionDateTo &&
                t.MaturityDate > transactionDateTo &&
                t.BondTypeId == ((int)BondTypesEnum.IslamicTreasury)
            );

            if (transactionDateFrom.HasValue)
            {
                filteredQuery = filteredQuery.Where(t => t.TransactionDate >= transactionDateFrom.Value);
            }

            var grandTotal = await filteredQuery.SumAsync(x => x.InvestmentPrice + x.Commission);

            var groupedQuery = filteredQuery.GroupBy(
                t => t.TransactionDate,
                (transactionDate, group) => new InvestmentReportItem
                {
                    TransactionDate = transactionDate,
                    TotalInvestmentAmount = group.Sum(x => x.InvestmentPrice + x.Commission)
                });

            var items = await groupedQuery
                .OrderBy(item => item.TransactionDate)
                .ToListAsync();
            return new InvestmentReportResult
            {
                GrandTotal = grandTotal,
                Items = items
            };
        }

        public async Task<CashInflowReportResult> GetCashInflowReportAsync(
            DateOnly? dateFrom,
            DateOnly? dateTo)
        {
            var ownedBonds = _context.Transactions
                .AsNoTracking()
                .Include(t => t.BondsEntity)
                .Where(t =>
                    t.TransactionType.Trim().ToUpper() == "BUY" &&
                    t.Status == 2 &&
                  
                    t.BondsEntity.TypeID == (int)BondTypesEnum.IslamicTreasury
                );

            var couponsQuery = ownedBonds
                .SelectMany(t => t.BondsEntity.CouponPayments, (t, cp) => new {
                    Transaction = t,
                    CouponPayment = cp
                });

            var maturitiesQuery = ownedBonds;

            if (dateFrom.HasValue)
            {
                couponsQuery = couponsQuery.Where(x => x.CouponPayment.PaymentDate >= dateFrom.Value);
                maturitiesQuery = maturitiesQuery.Where(t => t.BondsEntity.MaturityDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                couponsQuery = couponsQuery.Where(x => x.CouponPayment.PaymentDate <= dateTo.Value);
                maturitiesQuery = maturitiesQuery.Where(t => t.BondsEntity.MaturityDate <= dateTo.Value);
            }


            var coupons = couponsQuery
                .Select(x => new {
                    Symbol = x.Transaction.BondsEntity.Symbol,
                    MaturityDate = x.Transaction.BondsEntity.MaturityDate,
                    PaymentDate = x.CouponPayment.PaymentDate,
                    Amount = x.Transaction.Quantity * x.CouponPayment.AmountPerUnit,
                    PaymentType = "Coupon"
                });

            var maturities = maturitiesQuery
                .Select(t => new {
                    Symbol = t.BondsEntity.Symbol,
                    MaturityDate = t.BondsEntity.MaturityDate,
                    PaymentDate = t.BondsEntity.MaturityDate,
                    Amount = t.Quantity * t.BondsEntity.FaceValue,
                    PaymentType = "Maturity"
                });

            var allInflows = coupons.Concat(maturities);
            var allInflowsList = await allInflows.ToListAsync();
            var grandTotal = allInflowsList.Sum(x => x.Amount);

            var finalReport = allInflowsList
                .GroupBy(x => new {
                    x.Symbol,
                    x.MaturityDate,
                    x.PaymentDate,
                    x.PaymentType
                })
                .Select(g => new CashInflowReportItem
                {
                    Symbol = g.Key.Symbol,
                    MaturityDate = g.Key.MaturityDate,
                    PaymentDate = g.Key.PaymentDate,
                    PaymentType = g.Key.PaymentType,
                    TotalAmount = g.Sum(i => i.Amount)
                })
                .OrderBy(r => r.PaymentDate)
                .ToList();

            return new CashInflowReportResult
            {
                GrandTotal = grandTotal,
                Items = finalReport
            };
        }

        public async Task<BondAndTransactionSummaryDto> GetBondAndTransactionSummaryAsync(DateOnly dateOnly)
        {

            // 1️⃣ Latest Mandeh Transaction
            var latestDate = await _context.MndehTransactions
                .Where(m => m.Taeed == 2 && m.TransactionDate <= dateOnly)
                .MaxAsync(m => m.TransactionDate);

            var latestTime = await _context.MndehTransactions
                .Where(m => m.Taeed == 2 && m.TransactionDate == latestDate)
                .MaxAsync(m => m.TransactionTime);

            var latestTransactions = await _context.MndehTransactions
                .Where(m => m.Taeed == 2
                    && m.TransactionDate == latestDate
                    && m.TransactionTime == latestTime)
                .Select(m => new { m.Mablagh, m.DarRah })
                .FirstOrDefaultAsync();

            // IslamicTreasury Bond Summary
            var khazaneh = _context.Bonds
                .Where(b => b.TypeID == (int)BondTypesEnum.IslamicTreasury && b.MaturityDate > dateOnly);

            var khazanehKharid = _context.Transactions
                .Where(t => t.Status == (int)TransactionStatusEnum.Approved && t.TransactionDate <= dateOnly);

            var totalSumKhazaneKharid = await (
                from kh in khazaneh
                join khk in khazanehKharid on kh.ID equals khk.BondId
                select khk.InvestmentPrice + khk.Commission
            ).SumAsync();

            // GovernmentBond Bond Summary
            var ejareDolat = _context.Bonds
             .Where(b => b.TypeID == (int)BondTypesEnum.GovernmentBond && b.MaturityDate > dateOnly);

            var ejareDolatKharid = _context.Transactions
                .Where(t => t.Status == (int)TransactionStatusEnum.Approved && t.TransactionDate <= dateOnly);

            var totalSumeEjareDolatKharid = await (
                from kh in ejareDolat
                join khk in ejareDolatKharid on kh.ID equals khk.BondId
                select khk.InvestmentPrice + khk.Commission
            ).SumAsync();

            // PartnershipBond Bond Summary
            var PartnershipBond = _context.Bonds
             .Where(b => b.TypeID == (int)BondTypesEnum.PartnershipBond && b.MaturityDate > dateOnly);

            var PartnershipBondKharid = _context.Transactions
                .Where(t => t.Status == (int)TransactionStatusEnum.Approved && t.TransactionDate <= dateOnly);

            var totalSumePartnershipBondKharid = await (
                from kh in PartnershipBond
                join khk in PartnershipBondKharid on kh.ID equals khk.BondId
                select khk.InvestmentPrice + khk.Commission
            ).SumAsync();

            // Return combined result
            return new BondAndTransactionSummaryDto()
            {
                IncomeConcentrationAccountBalanceWithTheCentralBank = latestTransactions.Mablagh,
                ChecksInTransit = latestTransactions.DarRah,
                InvestmentBalance = totalSumePartnershipBondKharid + totalSumeEjareDolatKharid + totalSumKhazaneKharid,
                TotalSum = totalSumePartnershipBondKharid + totalSumeEjareDolatKharid + totalSumKhazaneKharid + ((decimal)latestTransactions.Mablagh),
                CanbekeptwiththeCentralBank30 = ((decimal)(latestTransactions.Mablagh)) * 30 / 100,
                Investable70 = ((decimal)(latestTransactions.Mablagh)) * 70 / 100
            }; 
        }

    }
}
