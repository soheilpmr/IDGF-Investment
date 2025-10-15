using Azure.Core;
using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Domain;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
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
        [HttpDelete(nameof(Delete))]
        public async Task<IActionResult> Delete(int id)
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
        [HttpPost(nameof(PostWithExcel))]
        public async Task<IActionResult> PostWithExcel(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var transactions = new List<MandehTransactions>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0]; // First sheet
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // skip header row
                        {
                            var transaction = new MandehTransactions
                            {
                                TransactionDateTime = worksheet.Cells[row, 1].GetValue<DateTime>(),
                                TransactionDate = DateOnly.FromDateTime(worksheet.Cells[row, 1].GetValue<DateTime>()),
                                TransactionTime = TimeOnly.FromDateTime(worksheet.Cells[row, 1].GetValue<DateTime>()),
                                Mablagh = worksheet.Cells[row, 2].GetValue<double>(),
                                DarRah = worksheet.Cells[row, 3].GetValue<double>(),
                                Taeed = worksheet.Cells[row, 4].GetValue<byte>(),
                                Hazineh = worksheet.Cells[row, 5].GetValue<double?>()
                            };
                            transactions.Add(transaction);
                        }
                    }
                }

                var rtnCount = await _mandehtransactionService.AddMutipleAsync(transactions);

                return Ok(new { Count = rtnCount, Message = "Records inserted successfully" });
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
        public async Task<IActionResult> Put(MandehPutDto mandeh)
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
        [Route(nameof(GetAllMandeh))]
        public async Task<ActionResult<List<MandehGetDto>>> GetAllMandeh()
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

        [HttpPost]
        [Route(nameof(GetAllMandehPagination))]
        public async Task<ActionResult<LinqDataResult<MandehGetDto>>> GetAllMandehPagination()
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _mandehtransactionService.ItemsAsync(request);
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
        [Route(nameof(GetAllMandehWithBodyRequest))]
        public async Task<ActionResult<LinqDataResult<MandehGetDto>>> GetAllMandehWithBodyRequest([FromBody] LinqDataRequest? linqDataRequest)
        {
            try
            {
                var res2 = await _mandehtransactionService.ItemsAsync(linqDataRequest);
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
