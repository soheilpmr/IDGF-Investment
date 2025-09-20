using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Domain;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.TimeZoneInfo;

namespace IDGF.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MandehController : ControllerBase
    {
        private readonly MandehtransactionService _mandehtransactionService;
        public MandehController(MandehtransactionService mandehtransactionService)
        {
            _mandehtransactionService = mandehtransactionService;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _mandehtransactionService.RemoveByIdAsync(id);
                return Ok();
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


        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("Put")]
        public async Task<ActionResult> Put(MandehPutDto mandeh)
        {
            var dbLoadedObject = await _mandehtransactionService.RetrieveByIdAsync(mandeh.ID);
            if (dbLoadedObject == null)
            {
                return StatusCode(500, "Code:6 Company object not found: companyID=" + mandeh.ID);
            }
            MandehTransactions domain = mandeh.Getmandeh();

            if (!User.IsInRole("Admin"))
            {
                throw new Exception("Code5: Cutomer users can just change the data related to their company");
            }
            try
            {

                domain.DarRah = mandeh.DarRah;
                domain.ID = mandeh.ID;
                await _mandehtransactionService.ModifyAsync(domain);
                return Ok("Mandeh Update Successfully");
            }
            catch (ServiceException ex)
            {
                if (ex is ServiceModelValidationException)
                {
                    return BadRequest(ex.Message + ", " + (ex as ServiceModelValidationException).JSONFormattedErrors);
                }

                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route(nameof(GetAll))]
        public async Task<ActionResult<List<MandehGetDto>>> GetAll()
        {
            try
            {
                var res = await _mandehtransactionService.ItemsAsync();
                return Ok(res);
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

    public class MandehPutDto
    {
        public long ID { get; set; }
        public double Mablagh { get; set; }
        public double DarRah { get; set; }
        public byte Taeed { get; set; }
        public MandehTransactions Getmandeh()
        {
            MandehTransactions mandehtransactions = new MandehTransactions();
            mandehtransactions.Mablagh = Mablagh;
            mandehtransactions.DarRah = DarRah;
            mandehtransactions.Taeed = Taeed;
            return mandehtransactions;
        }
    }

    public class MandehGetDto
    {
        public long ID { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public DateOnly TransactionDate { get; set; }
        public TimeOnly TransactionTime { get; set; }
        public double Mablagh { get; set; }
        public double DarRah { get; set; }
        public short Taeed { get; set; }
        public double Hazineh { get; set; }
        public MandehGetDto GetDto(MandehTransactions mandehTransactions)
        {
            MandehGetDto mandehGetDto = new MandehGetDto();
            mandehGetDto.Mablagh = mandehTransactions.Mablagh;
            mandehGetDto.DarRah = mandehTransactions.DarRah;
            mandehGetDto.Taeed = mandehTransactions.Taeed;
            mandehGetDto.TransactionDate = mandehTransactions.TransactionDate;
            mandehGetDto.TransactionTime = mandehTransactions.TransactionTime;
            mandehGetDto.TransactionDateTime = mandehTransactions.TransactionDateTime;
            mandehGetDto.Mablagh = mandehTransactions.Mablagh;
            mandehGetDto.ID = mandehTransactions.ID;
            return mandehGetDto;
        }
    }
}
