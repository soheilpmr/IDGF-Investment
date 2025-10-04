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


        [HttpPost]
        [Route(nameof(GetAllIslamicTreasury))]
        public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllIslamicTreasury()
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _bondsService.AllIslamicTreasuryItemsAsync(request);
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

        [HttpPost]
        [Route(nameof(GetAllIslamicTreasuryWithBodyRequest))]
        public async Task<IActionResult> GetAllIslamicTreasuryWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        {
            try
            {
                var res2 = await _bondsService.AllIslamicTreasuryItemsAsync(linqDataRequest);
                return Ok(res2);
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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(GetAllCouponIslamic))]
        public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllCouponIslamic()
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _bondsService.AllCouponIslamicTreasuryItemsAsync(request);
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

        [HttpPost]
        [Route(nameof(GetAllCouponIslamicWithBodyRequest))]
        public async Task<IActionResult> GetAllCouponIslamicWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        {
            try
            {
                var res2 = await _bondsService.AllCouponIslamicTreasuryItemsAsync(linqDataRequest);
                return Ok(res2);
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








        [HttpPost]
        [Route(nameof(GetAllGovernmentBondItemsAsync))]
        public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllGovernmentBondItemsAsync()
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _bondsService.AllGovernmentBondItemsAsync(request);
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

        [HttpPost]
        [Route(nameof(GetAllGovernmentBondWithBodyRequest))]
        public async Task<IActionResult> GetAllGovernmentBondWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        {
            try
            {
                var res2 = await _bondsService.AllGovernmentBondItemsAsync(linqDataRequest);
                return Ok(res2);
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







        [HttpPost]
        [Route(nameof(GetAllMurabahaBondItemsItemsAsync))]
        public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllMurabahaBondItemsItemsAsync()
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _bondsService.AllMurabahaBondItemsAsync(request);
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

        [HttpPost]
        [Route(nameof(GetAllMurabahaBondItemsWithBodyRequest))]
        public async Task<IActionResult> GetAllMurabahaBondItemsWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        {
            try
            {
                var res2 = await _bondsService.AllMurabahaBondItemsAsync(linqDataRequest);
                return Ok(res2);
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





        [HttpPost]
        [Route(nameof(GetAllPartnershipBondItemsAsync))]
        public async Task<ActionResult<LinqDataResult<BondsGetDto>>> GetAllPartnershipBondItemsAsync()
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _bondsService.AllPartnershipBondItemsAsync(request);
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

        [HttpPost]
        [Route(nameof(GetAllPartnershipBondItemsWithBodyRequest))]
        public async Task<IActionResult> GetAllPartnershipBondItemsWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        {
            try
            {
                var res2 = await _bondsService.AllPartnershipBondItemsAsync(linqDataRequest);
                return Ok(res2);
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
    }
   

}
