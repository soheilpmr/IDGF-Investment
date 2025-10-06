using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using BackEndInfrastructure.Infrastructure.Exceptions;
using BackEndInfrastructure.Infrastructure.Service;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.UnitOfWork;

namespace IDGF.Core.Services
{
    public class BondsService : StorageBusinessService<Bonds, decimal>
    {
        private readonly ICoreUnitOfWork _coreUnitOfWork;
        private const int _serviceLogNumber = 200;
        private readonly ILDRCompatibleRepositoryAsync<Bonds, decimal> _baseRepo;
        public BondsService(ICoreUnitOfWork coreUnitOfWork, ILogger<Bonds> logger) : base(logger, _serviceLogNumber)
        {
            _coreUnitOfWork = coreUnitOfWork;
            _baseRepo = _coreUnitOfWork.GetRepo<Bonds, decimal>();
        }

        public override Task<decimal> AddAsync(Bonds item)
        {
            throw new NotImplementedException();
        }

        public override Task<LinqDataResult<Bonds>> ItemsAsync(LinqDataRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BondsGetDto>> GetAllWithType(int typeID)
        {
            try
            {
                //var f = await _baseRepo.AllItemsAsync(request);
                var result = await _coreUnitOfWork.BondsRP.GetAllWithType(typeID);
                LogRetrieveMultiple();
                return result;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, null, ex);
                throw new ServiceStorageException($"Error retrieving the GetAllBonds list with Type : {typeID}", ex, _serviceLogNumber);
            }
        }

        public async Task<LinqDataResult<BondsGetDto>> AllIslamicTreasuryItemsAsync(LinqDataRequest request)
        {
            try
            {
                //var f = await _baseRepo.AllItemsAsync(request);
                var result = await _coreUnitOfWork.BondsRP.AllIslamicTreasuryItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return result;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the IslamicTreasury list ", ex, _serviceLogNumber);
            }
        }

        public async Task<LinqDataResult<Bonds>> AllCouponIslamicTreasuryItemsAsync(LinqDataRequest request)
        {
            try
            {
                //var f = await _baseRepo.AllItemsAsync(request);
                var f = await _coreUnitOfWork.BondsRP.AllCouponIslamicTreasuryItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return f;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the CouponIslamicTreasury list ", ex, _serviceLogNumber);
            }
        }

        public async Task<LinqDataResult<Bonds>> AllGovernmentBondItemsAsync(LinqDataRequest request)
        {
            try
            {
                //var f = await _baseRepo.AllItemsAsync(request);
                var f = await _coreUnitOfWork.BondsRP.AllGovernmentBondItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return f;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the GovernmentBond list ", ex, _serviceLogNumber);
            }
        }

        public async Task<LinqDataResult<Bonds>> AllMurabahaBondItemsAsync(LinqDataRequest request)
        {
            try
            {
                //var f = await _baseRepo.AllItemsAsync(request);
                var f = await _coreUnitOfWork.BondsRP.AllMurabahaBondItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return f;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the MurabahaBond list ", ex, _serviceLogNumber);
            }
        }

        public async Task<LinqDataResult<Bonds>> AllPartnershipBondItemsAsync(LinqDataRequest request)
        {
            try
            {
                //var f = await _baseRepo.AllItemsAsync(request);
                var f = await _coreUnitOfWork.BondsRP.AllPartnershipBondItemsAsync(request);
                LogRetrieveMultiple(null, request);
                return f;
            }
            catch (Exception ex)
            {
                LogRetrieveMultiple(null, request, ex);
                throw new ServiceStorageException("Error retrieving the PartnershipBond list ", ex, _serviceLogNumber);
            }
        }

        public override Task ModifyAsync(Bonds item)
        {
            throw new NotImplementedException();
        }

        public override Task RemoveByIdAsync(decimal ID)
        {
            throw new NotImplementedException();
        }

        public override Task<Bonds> RetrieveByIdAsync(decimal ID)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnAddAsync(Bonds item)
        {
            throw new NotImplementedException();
        }

        protected override Task ValidateOnModifyAsync(Bonds recievedItem, Bonds storageItem)
        {
            throw new NotImplementedException();
        }
    }
}
