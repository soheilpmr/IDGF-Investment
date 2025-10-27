using BackEndInfrastructure.DynamicLinqCore;
using BackEndInfrastructure.Infrastructure.Exceptions;
using IDGF.Core.Controllers.Dtos;
using IDGF.Core.Domain;
using IDGF.Core.Infrastructure.Repositories.Interface;
using IDGF.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace IDGF.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetingFileController : ControllerBase
    {
        private readonly MeetingFilesService _meetingFilesService;

        private readonly IMeetingBusinessService _meetingService;
        private readonly ILogger<MeetingFileController> _logger;

        public MeetingFileController(IMeetingBusinessService meetingService, ILogger<MeetingFileController> logger)
        {
            _meetingService = meetingService;
            _logger = logger;
        }

        [HttpPost]
        [Route(nameof(CreateMeeting))]
        public async Task<IActionResult> CreateMeeting([FromForm] CreateMeetingRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newMeetingId = await _meetingService.CreateMeetingWithFilesAsync(request);

                return CreatedAtAction(nameof(GetMeeting), new { id = newMeetingId }, new { id = newMeetingId });
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error while creating meeting.");
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error while creating meeting.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route(nameof(GetMeetingList))]
        public async Task<ActionResult<LinqDataResult<MeetingResponseDto>>> GetMeetingList()
        {
            try
            {
                var request = await Request.ToLinqDataHttpPostRequest();
                var result = await _meetingService.GetMeetingListAsync(request);
                return Ok(result);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error while getting meeting list.");
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error while getting meeting list.");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route(nameof(GetMeeting))]
        public async Task<ActionResult<Meeting>> GetMeeting(int id)
        {
            try
            {
                var result = await _meetingService.GetMeetingByIdAsync(id);
                return Ok(result);
            }
            catch (ServiceObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Meeting {ID} not found.", id);
                return NotFound(ex.Message);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error while getting meeting {ID}.", id);
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error while getting meeting {ID}.", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route(nameof(DeleteMeeting))]
        public async Task<IActionResult> DeleteMeeting(int id)
        {
            try
            {
                await _meetingService.RemoveMeetingAndFilesAsync(id);
                return NoContent();
            }
            catch (ServiceObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "Meeting {ID} not found for deletion.", id);
                return NotFound(ex.Message);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error while deleting meeting {ID}.", id);
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error while deleting meeting {ID}.", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route(nameof(DownloadFile))]
        public async Task<IActionResult> DownloadFile(int meetingId, string fileType)
        {
            try
            {
                var (stream, contentType, fileName) = await _meetingService.GetFileForDownloadAsync(meetingId, fileType);

                return File(stream, contentType, fileName);
            }
            catch (ServiceObjectNotFoundException ex)
            {
                _logger.LogWarning(ex, "File not found for meeting {MeetingID}, type {FileType}.", meetingId, fileType);
                return NotFound(ex.Message);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, "Service error while downloading file for meeting {MeetingID}.", meetingId);
                return StatusCode(500, ex.ToServiceExceptionString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error while downloading file for meeting {MeetingID}.", meetingId);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
