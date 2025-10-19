using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Infrastructure;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IDGF.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly TransactionService _transactionService;
        public ReportController(TransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        [Route(nameof(GetAllTransactions))]
        public async Task<ActionResult<LinqDataResult<TransactionResult>>> GetAllTransactions(int? bondId = null,
        int? brokerId = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null)
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _transactionService.GetAllTransactionViewService(request, bondId, brokerId, transactionDateFrom, transactionDateTo);
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
        [Route(nameof(GetAggregatedTransactionReport))]
        public async Task<ActionResult<LinqDataResult<AggregatedTransactionReportItem>>> GetAggregatedTransactionReport(int? bondId = null,
        int? brokerId = null,
        DateOnly? transactionDateFrom = null,
        DateOnly? transactionDateTo = null)
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var res1 = await _transactionService.GetAggregatedReportService(request, bondId, brokerId, transactionDateFrom, transactionDateTo);
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

        [HttpPost(nameof(ExportAggregatedTransactionReport))]
        public async Task<IActionResult> ExportAggregatedTransactionReport(
            int? bondId = null,
            int? brokerId = null,
            DateOnly? transactionDateFrom = null,
            DateOnly? transactionDateTo = null)
        {
            try
            {
                var fileBytes = await _transactionService.ExportAggregatedReportService(
                    bondId,
                    brokerId,
                    transactionDateFrom,
                    transactionDateTo);

                string fileName = $"AggregatedReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
