using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;

namespace IDGF.Core.Services
{
    public class BrokerageService : StorageBusinessService<Brokerage, int>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 400;
        public BrokerageService(ILogger<Brokerage> logger, ICoreUnitOfWork coreUnitOfWork):base(logger, _serviceLogNumber)
        {
            _coreUnitOfWork = coreUnitOfWork;
        }
        public override Task<int> AddAsync(Brokerage item)
        {
            throw new NotImplementedException();
        }

        public override Task<LinqDataResult<Brokerage>> ItemsAsync(LinqDataRequest request)
        {
            throw new NotImplementedException();
        }

        public override Task ModifyAsync(Brokerage item)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveByIdAsync(int ID)
        {
            throw new NotImplementedException();
        }

        public override Task<Brokerage> RetrieveByIdAsync(int ID)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnAddAsync(Brokerage item)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnModifyAsync(Brokerage recievedItem, Brokerage storageItem)
        {
            throw new NotImplementedException();
        }
    }
}
