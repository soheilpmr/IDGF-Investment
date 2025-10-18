using Azure.Core;
using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Data;
using IDGF.Core.Data.Entities;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Enums;
using IDGF.Core.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace IDGF.Core.Infrastructure.Repositories.Implemention
{
    public class BondsRepository : LDRCompatibleRepositoryAsync<BondsEntity, Bonds, decimal>, IBondsRepository
    {
        private readonly CoreDbContext _context;
        public BondsRepository(CoreDbContext coreDbContext) : base(coreDbContext)
        {
            _context = coreDbContext;
        }
        /// <summary>
        /// لیست اوراق بهادار اسلامی خزانه
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LinqDataResult<BondsGetDto>> AllIslamicTreasuryItemsAsync(LinqDataRequest request)
        {
            return await _context.Bonds
                .Where(ss => ss.TypeID == ((int)BondTypesEnum.IslamicTreasury))
                .Select(ss => new BondsGetDto
                {
                    Symbol = ss.Symbol,
                    MaturityDate = ss.MaturityDate,
                    FaceValue = ss.FaceValue
                })
                .OrderByDescending(ss => ss.MaturityDate)
               .ToLinqDataResultAsync<BondsGetDto>(request.Take, request.Skip, request.Sort, request.Filter);
        }
        /// <summary>
        /// لیست اوراق بهادار خزانه کوپن خزانه
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LinqDataResult<Bonds>> AllCouponIslamicTreasuryItemsAsync(LinqDataRequest request)
        {
            return await _context.Bonds
                .Where(ss => ss.TypeID == ((int)BondTypesEnum.CoponTreasury))
                .OrderByDescending(ss => ss.MaturityDate)
               .ToLinqDataResultAsync<Bonds>(request.Take, request.Skip, request.Sort, request.Filter);
        }

        /// <summary>
        /// اوراق اجاره دولت
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LinqDataResult<Bonds>> AllGovernmentBondItemsAsync(LinqDataRequest request)
        {
            return await _context.Bonds
                .Where(ss => ss.TypeID == ((int)BondTypesEnum.GovernmentBond))
                .OrderByDescending(ss => ss.MaturityDate)
               .ToLinqDataResultAsync<Bonds>(request.Take, request.Skip, request.Sort, request.Filter);
        }
        /// <summary>
        /// اوراق مرابحه
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LinqDataResult<Bonds>> AllMurabahaBondItemsAsync(LinqDataRequest request)
        {
            return await _context.Bonds
                .Where(ss => ss.TypeID == ((int)BondTypesEnum.MurabahaBond))
                .OrderByDescending(ss => ss.MaturityDate)
               .ToLinqDataResultAsync<Bonds>(request.Take, request.Skip, request.Sort, request.Filter);
        }
        /// <summary>
        /// اوراق مشارکت    
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LinqDataResult<Bonds>> AllPartnershipBondItemsAsync(LinqDataRequest request)
        {
            return await _context.Bonds
                .Where(ss => ss.TypeID == ((int)BondTypesEnum.PartnershipBond))
                .OrderByDescending(ss => ss.MaturityDate)
                .ToLinqDataResultAsync<Bonds>(request.Take, request.Skip, request.Sort, request.Filter);
        }

        public async Task<List<BondsGetDto>> GetAllWithType(int? typeID)
        {
            if (typeID is null)
            {
                return await _context.Bonds
                    .Select(ss => new BondsGetDto
                    {
                        Symbol = ss.Symbol,
                        MaturityDate = ss.MaturityDate,
                        FaceValue = ss.FaceValue
                    })
                .OrderByDescending(ss => ss.MaturityDate)
                .ToListAsync<BondsGetDto>();
            }
            else
            {
                return await _context.Bonds
               .Where(ss => ss.TypeID == typeID)
                   .Select(ss => new BondsGetDto
                   {
                       Symbol = ss.Symbol,
                       MaturityDate = ss.MaturityDate,
                       FaceValue = ss.FaceValue
                   })
               .OrderByDescending(ss => ss.MaturityDate)
               .ToListAsync<BondsGetDto>();
            }
        }

        public async Task<LinqDataResult<BondsGetDto>> GetAllWithTypeWithPagination(LinqDataRequest request, int? typeID)
        {
            if (typeID is null)
            {
                return await _context.Bonds
                   .Select(ss => new BondsGetDto
                   {
                       Symbol = ss.Symbol,
                       MaturityDate = ss.MaturityDate,
                       FaceValue = ss.FaceValue
                   })
               .OrderByDescending(ss => ss.MaturityDate)
               .ToLinqDataResultAsync<BondsGetDto>(request.Take, request.Skip, request.Sort, request.Filter);
            }
            else
            {
                return await _context.Bonds
              .Where(ss => ss.TypeID == typeID)
                 .Select(ss => new BondsGetDto
                 {
                     Symbol = ss.Symbol,
                     MaturityDate = ss.MaturityDate,
                     FaceValue = ss.FaceValue
                 })
              .OrderByDescending(ss => ss.MaturityDate)
              .ToLinqDataResultAsync<BondsGetDto>(request.Take, request.Skip, request.Sort, request.Filter);
            }

        }

        public async Task<decimal> GetBondIdWithName(string name)
        {
            var rtn = await _context.Bonds.Where(ss => ss.Symbol == name).Select(ss => ss.ID).FirstOrDefaultAsync();
            return rtn;
        }
    }
}
