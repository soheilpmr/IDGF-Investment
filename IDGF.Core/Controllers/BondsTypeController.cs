using AutoMapper;
using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Domain;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace IDGF.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BondsTypeController : ControllerBase
    {
        private readonly BondsTypeService _bondsTypeService;
        private readonly IMapper _mapper;   
        public BondsTypeController(BondsTypeService bondsTypeService, IMapper mapper)
        {
            _bondsTypeService = bondsTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route(nameof(GetAll))]
        public async Task<ActionResult<List<BondsTypeGetDto>>> GetAll()
        {
            try
            {
                var res = await _bondsTypeService.AllItemsAsync();
                var dtoResult = _mapper.Map<List<BondsTypeGetDto>>(res);
                return Ok(dtoResult);
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

    public class BondsTypeGetDto
    {
        public int ID { get; set; }
        public string HasCoupon { get; set; }
        public string Name { get; set; }

        public BondsTypeGetDto GetDto(BondsType bondsType)
        {
            BondsTypeGetDto bondsTypeDto = new BondsTypeGetDto();
            bondsTypeDto.Name = bondsType.Name;
            bondsType.HasCoupon = true ? bondsTypeDto.HasCoupon == "دارد" : bondsTypeDto.HasCoupon == "ندارد";
            return bondsTypeDto;
        }
    }
}
