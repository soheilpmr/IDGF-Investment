using IdentityModel;
using IDGF.Core.Services;
using IDGF.Core.Services.WorkFlow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IDGF.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkFlowController : ControllerBase
    {
        private readonly IWorkflowService _workFlowService;
        private readonly ReportService _reportService;
        public WorkFlowController(IWorkflowService workFlowService, ReportService reportService)
        {
            _reportService = reportService;
            _workFlowService = workFlowService;
        }

        public string? UserName
        {
            get
            {
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    if (User.Identity is ClaimsIdentity claimsIdentity)
                    {
                        return claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    }
                }
                return null;

            }
        }

        [HttpPost(nameof(StartWorkFlowAsync))]
        public async Task<IActionResult> StartWorkFlowAsync(IFormFile file, string workflowname)
        {
            try
            {
                var reportFileID = await _reportService.SubmitReportToWorkFlow(file, UserName);
                await _workFlowService.StartWorkflowAsync(reportFileID, workflowname, UserName);
                return Ok(reportFileID);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost(nameof(PerformActionOnFlowAsync))]
        public async Task<IActionResult> PerformActionOnFlowAsync(int instanceID, string action, string comment)
        {
            try
            {
                var reportFileID = await _workFlowService.PerformActionAsync(instanceID, action, UserName, comment);
                return Ok(reportFileID);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
