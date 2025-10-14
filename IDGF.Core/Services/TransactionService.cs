using AutoMapper;
using Azure.Core;
using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Views;
using IDGF.Core.Infrastructure;
using IDGF.Core.Infrastructure.UnitOfWork;

namespace IDGF.Core.Services
{
    public class TransactionService : StorageBusinessService<Transactions, decimal>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 600;
        private readonly IMapper _mapper;

        public TransactionService(ILogger<Transactions> logger, ICoreUnitOfWork coreUnitOfWork, IMapper mapper) : base(logger, _serviceLogNumber)
        {
            _coreUnitOfWork = coreUnitOfWork;
            _mapper = mapper;
        }

        public override Task<decimal> AddAsync(Transactions item)
        {
            throw new NotImplementedException();
        }

        public override Task<LinqDataResult<Transactions>> ItemsAsync(LinqDataRequest request)
        {
            throw new NotImplementedException();
        }

        public override Task ModifyAsync(Transactions item)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveByIdAsync(decimal ID)
        {
            throw new NotImplementedException();
        }

        public override Task<Transactions> RetrieveByIdAsync(decimal ID)
        {
            throw new NotImplementedException();
        }

        public async Task<LinqDataResult<TransactionResult>> GetAllTransactionViewService(LinqDataRequest linqDataRequest, int? bondId = null,
        string? brokerName = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null)
        {
            try
            {
                var rtnView = await _coreUnitOfWork.TransactionRP.GetAllItemsView(linqDataRequest, bondId, brokerName, transactionDateFrom, transactionDateTo);

                var mappedData = _mapper.Map<IEnumerable<TransactionResult>>(rtnView.Data);

                LinqDataResult<TransactionResult> linqDataResult = new LinqDataResult<TransactionResult>
                {
                    Data = mappedData,
                    RecordsTotal = rtnView.RecordsTotal,
                    RecordsFiltered = rtnView.RecordsFiltered   
                };

                return linqDataResult;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, linqDataRequest, ex);
                throw new ServiceStorageException("Error retrieving the TransactionView list ", ex, _serviceLogNumber);
            }
        }
        
        public async Task<LinqDataResult<AggregatedTransactionReportItem>> GetAggregatedReportService(LinqDataRequest linqDataRequest, int? bondId = null,
        int? brokerId = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null)
        {
            try
            {
                var result = await _coreUnitOfWork.TransactionRP.GetAggregatedTransactionReportAsync(
                    linqDataRequest,
                    bondId,
                    brokerId,
                    transactionDateFrom,
                    transactionDateTo);

                return result;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, linqDataRequest, ex);
                throw new ServiceStorageException("Error retrieving the TransactionView list ", ex, _serviceLogNumber);
            }
        }

        protected override Task ValidateOnAddAsync(Transactions item)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnModifyAsync(Transactions recievedItem, Transactions storageItem)
        {
            throw new NotImplementedException();
        }
    }
}
