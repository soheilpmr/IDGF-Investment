using BackEndInfrastructure.DynamicLinqCore;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Domain;

namespace IDGF.Core.Infrastructure.Repositories.Interface
{
    public interface IBondsRepository
    {
        Task<LinqDataResult<BondsGetDto>> AllIslamicTreasuryItemsAsync(LinqDataRequest request);
        Task<LinqDataResult<Bonds>> AllCouponIslamicTreasuryItemsAsync(LinqDataRequest request);
        Task<LinqDataResult<Bonds>> AllGovernmentBondItemsAsync(LinqDataRequest request);
        Task<LinqDataResult<Bonds>> AllMurabahaBondItemsAsync(LinqDataRequest request);
        Task<LinqDataResult<Bonds>> AllPartnershipBondItemsAsync(LinqDataRequest request);

        Task<List<BondsGetDto>> GetAllWithType(int typeID);
        Task<LinqDataResult<BondsGetDto>> GetAllWithTypeWithPagination(LinqDataRequest request, int typeID);

    }
}
