using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;

namespace IDGF.Core.Services
{
    public class TransactionService : StorageBusinessService<Transactions, decimal>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 500;

        public TransactionService(ILogger<Transactions> logger, ICoreUnitOfWork coreUnitOfWork) : base(logger, _serviceLogNumber)   
        {
            _coreUnitOfWork = coreUnitOfWork;
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
