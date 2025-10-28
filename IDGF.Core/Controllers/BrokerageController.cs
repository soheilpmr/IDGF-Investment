using AutoMapper;
using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Domain;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace IDGF.Core.Controllers
{
    [ApiController]
    [Route("api/brokerages")]
    public class BrokerageController : ControllerBase
    {
        private readonly BrokerageService _brokerageService;
        private readonly ILogger<BrokerageController> _logger;
        private readonly IMapper _mapper;

        public BrokerageController(
        BrokerageService brokerageService,
        ILogger<BrokerageController> logger,
        IMapper mapper)
        {
            _brokerageService = brokerageService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost(nameof(GetAllBrokeragesPagination))]
        public async Task<ActionResult<LinqDataResult<Brokerage>>> GetAllBrokeragesPagination()
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var result = await _brokerageService.ItemsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving brokerages.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }



        [HttpGet(nameof(GetAllBrokerages))]
        public async Task<ActionResult<List<BrokerageGetDto>>> GetAllBrokerages()
        {
            try
            {
                var result = await _brokerageService.ItemsAsync();
                var rtn = result.Select(ss => new BrokerageGetDto()
                {
                    Name = ss.Name,
                    ID = ss.ID
                });
                return Ok(rtn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving brokerages.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpGet("GetBrokerageById")]
        public async Task<ActionResult<Brokerage>> GetBrokerageById(int id)
        {
            try
            {
                var brokerage = await _brokerageService.RetrieveByIdAsync(id);
                return Ok(brokerage);
            }
            catch (ServiceObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Brokerage with ID {Id} not found.", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving brokerage with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost("CreateBrokerage")]
        public async Task<IActionResult> CreateBrokerage([FromBody] BrokerageCreateDto createDto)
        {
            try
            {
                var brokerage = _mapper.Map<Brokerage>(createDto);

                var Id = await _brokerageService.AddAsync(brokerage);

                return CreatedAtAction(nameof(GetBrokerageById), new { id = Id },  new { Id, createDto.Name});
            }
            catch (ServiceModelValidationException ex)
            {
                _logger.LogWarning("Validation failed while creating brokerage: {Errors}", ex.JSONFormattedErrors);
                return BadRequest(ex.JSONFormattedErrors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a brokerage.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPut("UpdateBrokerage")]
        public async Task<IActionResult> UpdateBrokerage(int id, [FromBody] BrokerageUpdateDto updateDto)
        {
            try
            {
                var brokerage = _mapper.Map<Brokerage>(updateDto);
                brokerage.ID = id;
                await _brokerageService.ModifyAsync(brokerage);

                return Ok($"Successfully Updated with ID : {id}");
            }
            catch (ServiceObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Brokerage with ID {Id} not found for update.", id);
                return NotFound(ex.Message);
            }
            catch (ServiceModelValidationException ex)
            {
                _logger.LogWarning("Validation failed while updating brokerage {Id}: {Errors}", id, ex.JSONFormattedErrors);
                return BadRequest(ex.JSONFormattedErrors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating brokerage with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpDelete("DeleteBrokerage")]
        public async Task<IActionResult> DeleteBrokerage(int id)
        {
            try
            {
                await _brokerageService.RemoveByIdAsync(id);
                return Ok($"Successfully Deleted with ID : {id}");
            }
            catch (ServiceObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Brokerage with ID {Id} not found for deletion.", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting brokerage with ID {Id}.", id);
                return StatusCode(500, "An internal server error occurred.");
            }
        }

    }
}
