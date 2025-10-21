using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Domain;
using IDGF.Core.Domain.Enums;
using IDGF.Core.Infrastructure.Repositories.Interface;
using IDGF.Core.Infrastructure.UnitOfWork;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using System.Data;
using System.Text.RegularExpressions;
using System.Transactions;

namespace IDGF.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionService transactionService;
        private readonly BondsService bondsService;
        public TransactionController(TransactionService transactionService, BondsService bondsService)
        {
             this.transactionService = transactionService;
            this.bondsService = bondsService;
        }

        /// <summary>
        /// import kargozari bank melli excel file  
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost(nameof(UploadMelliExcel))]
        public async Task<IActionResult> UploadMelliExcel(IFormFile file)
        {
            try
            {
                var extractedRows = await transactionService.UploadFileExcelMelli(file);
                await transactionService.AddMutipleAsync(extractedRows);

                return Ok(new
                {
                    Message = "✅ مقادیر با موفقیت استخراج و ذخیره شدند.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (ServiceException ex)
            {
                if(ex is UploadFileException)
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost(nameof(UploadMellatExcel))]
        public async Task<IActionResult> UploadMellatExcel(IFormFile file)
        {
            try
            {
                var extractedRows = await transactionService.UploadFileExcelMellat(file);
                await transactionService.AddMutipleAsync(extractedRows);

                return Ok(new
                {
                    Message = "✅ مقادیر با موفقیت استخراج و ذخیره شدند.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (ServiceException ex)
            {
                if (ex is UploadFileException)
                {
                    return BadRequest(ex.Message);
                }
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost(nameof(UploadKeshavarziExcel))]
        public async Task<IActionResult> UploadKeshavarziExcel(IFormFile file)
        {
            try
            {
                var extractedRows = await transactionService.UploadFileExcelKeshavarzi(file);
                await transactionService.AddMutipleAsync(extractedRows);

                return Ok(new
                {
                    Message = "✅ مقادیر با موفقیت استخراج و ذخیره شدند.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost(nameof(UploadSanatExcel))]
        public async Task<IActionResult> UploadSanatExcel(IFormFile file)
        {
            try
            {
                var extractedRows = await transactionService.UploadFileExcelSanat(file);
                await transactionService.AddMutipleAsync(extractedRows);


                return Ok(new
                {
                    Message = "✅ Bank Sanat va Madan file processed successfully.",
                    Count = extractedRows.Count,
                    Data = extractedRows
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost(nameof(ApproveMultiTask))]
        public async Task<IActionResult> ApproveMultiTask(List<decimal> Ids)
        {
            try
            {
                await transactionService.ApproveMultiTask(Ids);
                return Ok(new
                {
                    Message = "✅ تراکنش‌ها با موفقیت تایید شدند.",
                    Count = Ids.Count
                }); 
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost(nameof(RejectMultiTask))]
        public async Task<IActionResult> RejectMultiTask(List<decimal> Ids)
        {
            try
            {
                await transactionService.RejectMultiTask(Ids);
                return Ok(new
                {
                    Message = "X تراکنش‌ها با موفقیت رد شدند.",
                    Count = Ids.Count
                });
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}