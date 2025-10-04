using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;

namespace IDGF.Core.Services
{
    public class BondsTypeService : StorageBusinessService<BondsType, int>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 300;
        public BondsTypeService(ICoreUnitOfWork coreUnitOfWork, ILogger<BondsType> logger) : base(logger, _serviceLogNumber)
        {
                _coreUnitOfWork = coreUnitOfWork;
        }

        public override Task<int> AddAsync(BondsType item)
        {
            throw new NotImplementedException();
        }

        public override Task<LinqDataResult<BondsType>> ItemsAsync(LinqDataRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BondsType>> AllItemsAsync()
        {
            try
            {
                var f = await _coreUnitOfWork.BondsTypeRP.AllItemsAsync();
                LogRetrieveMultiple(null);
                return f.ToList();
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, null, ex);
                throw new ServiceStorageException("Error retrieving the BondsType list ", ex, _serviceLogNumber);
            }
        }


        public override Task ModifyAsync(BondsType item)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveByIdAsync(int ID)
        {
            throw new NotImplementedException();
        }

        public override Task<BondsType> RetrieveByIdAsync(int ID)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnAddAsync(BondsType item)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnModifyAsync(BondsType recievedItem, BondsType storageItem)
        {
            throw new NotImplementedException();
        }
    }
}
