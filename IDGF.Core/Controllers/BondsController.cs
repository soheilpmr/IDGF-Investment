using AutoMapper;
using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Domain;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDGF.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BondsController : ControllerBase
    {
        private readonly BondsService _bondsService;
        private readonly IMapper _mapper;
        public BondsController(IMapper mapper, BondsService bondsService)
        {
            _bondsService = bondsService;
            _mapper = mapper;
        }

        /// <summary>
        /// لیست تمام اوراق با نوع ورودی
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetAllTypes))]
        
        public async Task<ActionResult<List<BondsGetDto>>> GetAllTypes(int? typeID)
        {
            try
            {
                var res1 = await _bondsService.GetAllWithType(typeID);
                return Ok(res1);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// لیست تمام اوراق woth pagination
        /// </summary>
        /// <param name="typeID">نوع ورودی اوراق</param>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetAllTypesPagination))]
       
        public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllTypesPagination([FromQuery] int? typeID)
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _bondsService.GetAllWithTypeWithPagination(request, typeID);
                return Ok(res1);
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        ///// <summary>
        ///// لیست تمام اوراق اسلامی خزانه
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route(nameof(GetAllIslamicTreasury))]
   
        //public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllIslamicTreasury()
        //{
        //    try
        //    {
        //        var request = await Request.ToLinqDataHttpPostRequest();
        //        var res1 = await _bondsService.AllIslamicTreasuryItemsAsync(request);
        //        return Ok(res1);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Retrieves a collection of Islamic Treasury items based on the specified data request.
        ///// </summary>
        ///// <remarks>This method processes the request asynchronously and returns the results in an HTTP
        ///// response.  If an error occurs during processing, a 500 Internal Server Error response is returned.</remarks>
        ///// <param name="linqDataRequest">The data request containing filtering, sorting, and pagination options.  This parameter can be null, in
        ///// which case default options are applied.</param>
        ///// <returns>An <see cref="IActionResult"/> containing the collection of Islamic Treasury items  that match the specified
        ///// criteria, or an error response if the operation fails.</returns>
        //[HttpPost]
        //[Route(nameof(GetAllIslamicTreasuryWithBodyRequest))]
        //public async Task<IActionResult> GetAllIslamicTreasuryWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        //{
        //    try
        //    {
        //        var res2 = await _bondsService.AllIslamicTreasuryItemsAsync(linqDataRequest);
        //        return Ok(res2);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}






        ///// <summary>
        ///// Retrieves a filtered and paginated list of Islamic treasury bond coupon items.
        ///// </summary>
        ///// <remarks>This method processes an HTTP POST request to retrieve Islamic treasury bond coupon
        ///// items based on the filtering, sorting, and pagination criteria provided in the request body. The method
        ///// returns the results in a format compatible with LinqDataResult.</remarks>
        ///// <returns>An <see cref="ActionResult{T}"/> containing a <see cref="LinqDataResult{T}"/> of <see cref="BondsGetDto"/>
        ///// objects representing the filtered and paginated Islamic treasury bond coupon items.</returns>
        //[HttpPost]
        //[Route(nameof(GetAllCouponIslamic))]
        //public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllCouponIslamic()
        //{
        //    try
        //    {
        //        var request = await Request.ToLinqDataHttpPostRequest();
        //        var res1 = await _bondsService.AllCouponIslamicTreasuryItemsAsync(request);
        //        return Ok(res1);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        ///// <summary>
        ///// retrieves a collection of Islamic Treasury Coupon items based on the specified data request.
        ///// </summary>
        ///// <param name="linqDataRequest">he data request containing filtering, sorting, and pagination options.  This parameter can be null, in
        ///// which case default options are applied.</param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route(nameof(GetAllCouponIslamicWithBodyRequest))]
        //public async Task<IActionResult> GetAllCouponIslamicWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        //{
        //    try
        //    {
        //        var res2 = await _bondsService.AllCouponIslamicTreasuryItemsAsync(linqDataRequest);
        //        return Ok(res2);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}







        ///// <summary>
        ///// retrieves a collection of Government Bond items based on the specified data request.    
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route(nameof(GetAllGovernmentBondItemsAsync))]
        //public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllGovernmentBondItemsAsync()
        //{
        //    try
        //    {
        //        var request = await Request.ToLinqDataHttpPostRequest();
        //        var res1 = await _bondsService.AllGovernmentBondItemsAsync(request);
        //        return Ok(res1);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        ///// <summary>
        ///// retrieves a collection of Government Bond items based on the specified data request.
        ///// </summary>
        ///// <param name="linqDataRequest"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route(nameof(GetAllGovernmentBondWithBodyRequest))]
        //public async Task<IActionResult> GetAllGovernmentBondWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        //{
        //    try
        //    {
        //        var res2 = await _bondsService.AllGovernmentBondItemsAsync(linqDataRequest);
        //        return Ok(res2);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}





        ///// <summary>
        ///// retrieves a collection of Murabaha Bond items based on the specified data request.
        ///// </summary>
        ///// <returns></returns>

        //[HttpPost]
        //[Route(nameof(GetAllMurabahaBondItemsItemsAsync))]
        //public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllMurabahaBondItemsItemsAsync()
        //{
        //    try
        //    {
        //        var request = await Request.ToLinqDataHttpPostRequest();
        //        var res1 = await _bondsService.AllMurabahaBondItemsAsync(request);
        //        return Ok(res1);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        ///// <summary>
        ///// retrieves a collection of Murabaha Bond items based on the specified data request.
        ///// </summary>
        ///// <param name="linqDataRequest">he data request containing filtering, sorting, and pagination options.  This parameter can be null, in
        ///// which case default options are applied.</param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route(nameof(GetAllMurabahaBondItemsWithBodyRequest))]
        //public async Task<IActionResult> GetAllMurabahaBondItemsWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        //{
        //    try
        //    {
        //        var res2 = await _bondsService.AllMurabahaBondItemsAsync(linqDataRequest);
        //        return Ok(res2);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}




        ///// <summary>
        ///// retrieves a collection of Partnership Bond items based on the specified data request.
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route(nameof(GetAllPartnershipBondItemsAsync))]
        //public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllPartnershipBondItemsAsync()
        //{
        //    try
        //    {
        //        var request = await Request.ToLinqDataHttpPostRequest();
        //        var res1 = await _bondsService.AllPartnershipBondItemsAsync(request);
        //        return Ok(res1);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
        ///// <summary>
        ///// retrieves a collection of Partnership Bond items based on the specified data request.
        ///// </summary>
        ///// <param name="linqDataRequest">he data request containing filtering, sorting, and pagination options.  This parameter can be null, in
        ///// which case default options are applied.</param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route(nameof(GetAllPartnershipBondItemsWithBodyRequest))]
        //public async Task<IActionResult> GetAllPartnershipBondItemsWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        //{
        //    try
        //    {
        //        var res2 = await _bondsService.AllPartnershipBondItemsAsync(linqDataRequest);
        //        return Ok(res2);
        //    }
        //    catch (ServiceException ex)
        //    {
        //        return StatusCode(500, ex.ToServiceExceptionString());
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
    }
}
